using MicroAC.Core.Auth;
using MicroAC.Core.Persistence;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Fabric;
using System.Linq;
using Microsoft.Extensions.Primitives;
using MicroAC.Core.Common;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace MicroAC.Authorization
{
    [Route("/")]
    public class AuthorizationController : Controller
    {
        readonly StatelessServiceContext _serviceContext;

        readonly IJwtTokenHandler<AccessExternal> _accessExternalTokenHandler;

        readonly IJwtTokenHandler<AccessInternal> _accessInternalTokenHandler;

        readonly IClaimBuilder<AccessInternal> _accessInternalClaimBuilder;

        readonly IPermissionsRepository _permissionsRepository;

        readonly string _timestampHeader;

        readonly string _serviceName;

        public AuthorizationController(
            StatelessServiceContext serviceContext,
            IJwtTokenHandler<AccessExternal> accessExternalTokenHandler,
            IJwtTokenHandler<AccessInternal> accessInternalTokenHandler,
            IClaimBuilder<AccessInternal> accessInternalClaimBuiler,
            IPermissionsRepository permissionsRepository,
            IConfiguration config
            )
        {
            _serviceContext = serviceContext;
            _accessExternalTokenHandler = accessExternalTokenHandler;
            _accessInternalTokenHandler = accessInternalTokenHandler;
            _accessInternalClaimBuilder = accessInternalClaimBuiler;
            _permissionsRepository = permissionsRepository;
            _timestampHeader = config.GetSection("Timestamp:Header").Value;
            _serviceName = config.GetSection("Timestamp:ServiceName").Value;
        }

        [HttpGet]
        public ActionResult  Index()
        {
            return Ok($"Authorization is ok on {_serviceContext.NodeContext.NodeName} at {DateTime.Now}");
        }

        [HttpPost("Authorize")]
        public async Task<ActionResult> Authorize()
        {
            var hasToken = this.Request.Headers.TryGetValue("Authorization", out StringValues headerValues);
            if (!hasToken)
            {
                return UnauthorizedWithTimestamp("Missing external access token.");
            }

            var token = headerValues.FirstOrDefault();

            if (token is null)
            {
                return UnauthorizedWithTimestamp("Missing external access token.");
            }

            var accessClaims = _accessExternalTokenHandler.Validate(token);

            var roles = accessClaims.Claims.Where(c => c.Type == MicroACClaimTypes.Roles)
                                                           .Select(c => c.Value);

            if (roles.Count() < 1)
            {
                return UnauthorizedWithTimestamp("User has no roles assigned therefore cannot be authorized.");
            }

            this.HttpContext.AddActionMessage(_timestampHeader, _serviceName, "AuthStart");
            var permissions = await _permissionsRepository.GetRolePermissions(roles);

            var claims = _accessInternalClaimBuilder
                .AddCommonClaims()
                .AddRoles(roles)
                .AddSubjectClaims(permissions)
                .Build();
            var jwt = _accessInternalTokenHandler.Create(claims);
            
            this.HttpContext.AddActionMessage(_timestampHeader, _serviceName, "Success");

            return Ok(jwt);
        }

        private ActionResult UnauthorizedWithTimestamp(string reason)
        {
            this.HttpContext.AddActionMessage(_timestampHeader, _serviceName, "Unauthorized");
            return Unauthorized(reason);
        }
    }
}
