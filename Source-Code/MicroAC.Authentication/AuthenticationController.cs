using MicroAC.Core.Auth;
using MicroAC.Core.Models;
using MicroAC.Core.Persistence;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Fabric;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using MicroAC.Core.Common;
using Microsoft.Extensions.Configuration;
using MicroAC.Core.Exceptions;

namespace MicroAC.Authentication
{
    [Route("/")]
    public class AuthenticationController : Controller
    {
        readonly StatelessServiceContext _serviceContext;

        readonly IJwtTokenHandler<AccessExternal> _accessTokenHandler;

        readonly IClaimBuilder<AccessExternal> _accessClaimBuilder;

        readonly IJwtTokenHandler<RefreshExternal> _refreshTokenHandler;

        readonly IClaimBuilder<RefreshExternal> _refreshClaimBuilder;

        readonly IUsersRepository _usersRepository;

        readonly string _serviceName;

        public AuthenticationController(
            StatelessServiceContext serviceContext,
            IJwtTokenHandler<AccessExternal> accessTokenHandler,
            IClaimBuilder<AccessExternal> accessClaimBuiler,
            IJwtTokenHandler<RefreshExternal> refreshTokenHandler,
            IClaimBuilder<RefreshExternal> refreshClaimBuiler,
            IUsersRepository usersRepository,
            IConfiguration config
            )
        {
            _serviceContext = serviceContext;
            _accessTokenHandler = accessTokenHandler;
            _accessClaimBuilder = accessClaimBuiler;
            _refreshTokenHandler = refreshTokenHandler;
            _refreshClaimBuilder = refreshClaimBuiler;
            _usersRepository = usersRepository;
            _serviceName = config.GetSection("Timestamp:ServiceName").Value;
        }

        [HttpGet]
        public ActionResult Index()
        {
            return Ok($"Authentication is ok on {_serviceContext.NodeContext.NodeName} at {DateTime.Now}");
        }

        [HttpPost("Login")]
        public async Task<ActionResult> Login([FromBody] LoginCredentials credentials)
        {
            if (credentials == null || credentials.Email == null /* || credentials.Password == null*/)
            {
                return BadRequest("Login credentials not provided.");
            }
            try
            {
                this.HttpContext.AddActionMessage(_serviceName, "StartAuth");
                var user = await _usersRepository.GetUser(credentials.Email, credentials.Password);

                if (user == null)
                {
                    throw new AuthenticationFailedException("User not found.");
                }

                var accessClaims = _accessClaimBuilder
                 .AddCommonClaims()
                 .AddUserId(user.Id)
                 .AddRoles(user.Roles)
                 .Build();
                var accessJwt = _accessTokenHandler.Create(accessClaims);

                var refreshClaims = _refreshClaimBuilder
                 .AddCommonClaims()
                 .AddUserId(user.Id)
                 .Build();
                var refreshJwt = _accessTokenHandler.Create(refreshClaims);

                this.HttpContext.AddActionMessage(_serviceName, "Success");

                return Ok(new { accessJwt, refreshJwt });
            }
            catch (Exception e)
            {
                throw new AuthenticationFailedException("Incorrect username or password.", e);
            }
        }

        [HttpPost("Refresh")]
        public async Task<ActionResult> Refresh()
        {
            try
            {
                var bodyStream = new StreamReader(Request.Body);

                var refreshClaims = _refreshTokenHandler.Validate(await bodyStream.ReadToEndAsync());

                var userId = refreshClaims.Claims.FirstOrDefault(c => c.Type == MicroACClaimTypes.UserId)?.Value;
                var user = await _usersRepository.GetUser(new Guid(userId));

                var accessClaims = _accessClaimBuilder
                 .AddCommonClaims()
                 .AddUserId(user.Id)
                 .AddRoles(user.Roles)
                 .Build();
                var accessJwt = _accessTokenHandler.Create(accessClaims);

                return Ok(accessJwt);
            }
            catch (Exception e)
            {
                throw new AuthenticationFailedException("Cannot to issue refresh token", e);
            }
        }
    }
}
