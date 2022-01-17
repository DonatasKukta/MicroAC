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

        readonly string _timestampHeader;

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
            _timestampHeader = config.GetSection("Timestamp:Header").Value;
            _serviceName = config.GetSection("Timestamp:ServiceName").Value;
        }

        [HttpGet]
        public ActionResult Index()
        {
            return Ok($"Authentication is ok on {_serviceContext.NodeContext.NodeName} at {DateTime.Now}");
        }

        [HttpPost("Login")]
        public ActionResult Login([FromBody] LoginCredentials credentials)
        {
            if (credentials.Email == null /* || credentials.Password == null*/)
            {
                return Unauthorized("Login credentials not provided.");
            }

            this.HttpContext.AddActionMessage(_timestampHeader, _serviceName, "StartAuth");
            var user = _usersRepository.GetUser(credentials.Email, credentials.Password);
            
            if (user == null)
            {
                return Unauthorized("Incorrect username or password.");
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

            this.HttpContext.AddActionMessage(_timestampHeader, _serviceName, "Success");

            return Ok(new { accessJwt, refreshJwt });
        }

        [HttpPost("Refresh")]
        public async Task<ActionResult> Refresh()
        {
            var bodyStream = new StreamReader(Request.Body);

            var refreshClaims  = _refreshTokenHandler.Validate(await bodyStream.ReadToEndAsync());

            var userId = refreshClaims.Claims.FirstOrDefault(c => c.Type == MicroACClaimTypes.UserId)?.Value;
            var user = _usersRepository.GetUser(new Guid(userId));
            if (user == null)
            {
                return UnauthorizedWithTimestamp("User Id could not be found.");
            }

            var accessClaims = _accessClaimBuilder
             .AddCommonClaims()
             .AddUserId(user.Id)
             .AddRoles(user.Roles)
             .Build();
            var accessJwt = _accessTokenHandler.Create(accessClaims);

            return Ok(accessJwt);
        }

        private ActionResult UnauthorizedWithTimestamp(string reason)
        {
            this.HttpContext.AddActionMessage(_timestampHeader, _serviceName, "Unauthorized");
            return Unauthorized(reason);
        }
    }
}
