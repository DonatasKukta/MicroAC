﻿using MicroAC.Core.Auth;
using Microsoft.AspNetCore.Mvc;

namespace MicroAC.Authentication
{
    [Route("[controller]")]
    public class AuthorizationController : Controller
    {
        readonly IJwtTokenHandler<AccessInternal> _accessTokenHandler;
        readonly IClaimBuilder<AccessInternal> _accessClaimBuilder;

        public AuthorizationController(
            IJwtTokenHandler<AccessInternal> accessTokenHandler,
            IClaimBuilder<AccessInternal> accessClaimBuiler
            )
        {
            _accessTokenHandler = accessTokenHandler;
            _accessClaimBuilder = accessClaimBuiler;
        }

        [HttpGet]
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
