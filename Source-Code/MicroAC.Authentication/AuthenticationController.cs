using MicroAC.Core.Auth;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Fabric;

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

        public AuthenticationController(
            StatelessServiceContext serviceContext,
            IJwtTokenHandler<AccessExternal> accessTokenHandler,
            IClaimBuilder<AccessExternal> accessClaimBuiler,
            IJwtTokenHandler<RefreshExternal> refreshTokenHandler,
            IClaimBuilder<RefreshExternal> refreshClaimBuiler
            )
        {
            _serviceContext = serviceContext;
            _accessTokenHandler = accessTokenHandler;
            _accessClaimBuilder = accessClaimBuiler;
            _refreshTokenHandler = refreshTokenHandler;
            _refreshClaimBuilder = refreshClaimBuiler;
        }

        [HttpGet]
        public ActionResult Index()
        {
            return Ok($"Authentication is ok on {_serviceContext.NodeContext.NodeName} at {DateTime.Now}");
        }

        [HttpGet("Login")]
        public ActionResult Login()
        {
            var claims = _accessClaimBuilder
             .AddCommonClaims()
             .Build();
            var jwt = _accessTokenHandler.Create(claims);
            return Ok(jwt);
        }

        [HttpGet("Refresh")]
        public ActionResult Refresh()
        {
            var claims = _refreshClaimBuilder
                .AddCommonClaims()
                .Build();
            var jwt = _refreshTokenHandler.Create(claims);
            return Ok(jwt);
        }
    }
}
