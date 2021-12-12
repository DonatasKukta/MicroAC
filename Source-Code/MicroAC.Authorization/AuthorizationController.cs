using MicroAC.Core.Auth;
using MicroAC.Core.Persistence;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Fabric;
using System.IO;
using System.Threading.Tasks;
using System.Linq;

namespace MicroAC.Authentication
{
    [Route("/")]
    public class AuthorizationController : Controller
    {
        readonly StatelessServiceContext _serviceContext;

        readonly IJwtTokenHandler<AccessExternal> _accessExternalTokenHandler;

        readonly IJwtTokenHandler<AccessInternal> _accessInternalTokenHandler;
        readonly IClaimBuilder<AccessInternal> _accessInternalClaimBuilder;

        readonly IPermissionsRepository _permissionsRepository;

        public AuthorizationController(
            StatelessServiceContext serviceContext,
            IJwtTokenHandler<AccessExternal> accessExternalTokenHandler,
            IJwtTokenHandler<AccessInternal> accessInternalTokenHandler,
            IClaimBuilder<AccessInternal> accessInternalClaimBuiler,
            IPermissionsRepository permissionsRepository
            )
        {
            _serviceContext = serviceContext;
            _accessExternalTokenHandler = accessExternalTokenHandler;
            _accessInternalTokenHandler = accessInternalTokenHandler;
            _accessInternalClaimBuilder = accessInternalClaimBuiler;
            _permissionsRepository = permissionsRepository;
        }

        [HttpGet]
        public ActionResult  Index()
        {
            return Ok($"Authorization is ok on {_serviceContext.NodeContext.NodeName} at {DateTime.Now}");
        }

        [HttpPost("Authorize")]
        public async Task<ActionResult> Authorize()
        {
            var bodyStream = new StreamReader(Request.Body);
            var bodyStr = await bodyStream.ReadToEndAsync();
            var accessClaims = _accessExternalTokenHandler.Validate(bodyStr);

            var roles = accessClaims.Claims.Where(c => c.Type == MicroACClaimTypes.Roles)
                                                           .Select(c => c.Value);

            if (roles.Count() < 1)
                return Unauthorized("User has no roles assigned therefore cannot be authorized.");

            var permissions = _permissionsRepository.GetRolePermissions(roles);

            var claims = _accessInternalClaimBuilder
                .AddCommonClaims()
                .AddRoles(roles)
                .AddSubjectClaims(permissions)
                .Build();
            var jwt = _accessInternalTokenHandler.Create(claims);

            return Ok(jwt);
        }
    }
}
