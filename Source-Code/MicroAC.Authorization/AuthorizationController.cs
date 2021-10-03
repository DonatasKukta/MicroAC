using MicroAC.Core.Auth;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Fabric;

namespace MicroAC.Authentication
{
    [Route("/")]
    public class AuthorizationController : Controller
    {
        readonly StatelessServiceContext _serviceContext;
        readonly IJwtTokenHandler<AccessInternal> _accessTokenHandler;
        readonly IClaimBuilder<AccessInternal> _accessClaimBuilder;

        public AuthorizationController(
            StatelessServiceContext serviceContext,
            IJwtTokenHandler<AccessInternal> accessTokenHandler,
            IClaimBuilder<AccessInternal> accessClaimBuiler
            )
        {
            _serviceContext = serviceContext;
            _accessTokenHandler = accessTokenHandler;
            _accessClaimBuilder = accessClaimBuiler;
        }

        [HttpGet]
        public ActionResult  Index()
        {
            return Ok($"Authorization is ok on {_serviceContext.NodeContext.NodeName} at {DateTime.Now}");
        }

        [HttpGet("Authorize")]
        public ActionResult Authorize()
        {
            var claims = _accessClaimBuilder
                .AddCommonClaims()
                .Build();
            var jwt = _accessTokenHandler.Create(claims);
            return Ok(jwt);
        }
    }
}
