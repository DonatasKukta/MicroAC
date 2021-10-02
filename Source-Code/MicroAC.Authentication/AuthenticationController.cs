using MicroAC.Core.Auth;
using Microsoft.AspNetCore.Mvc;


namespace MicroAC.Authentication
{
    [Route("[controller]")]
    public class AuthenticationController : Controller
    {
        readonly IJwtTokenHandler<AccessExternal> _accessTokenHandler;
        readonly IClaimBuilder<AccessExternal> _accessClaimBuilder;

        readonly IJwtTokenHandler<RefreshExternal> _refreshTokenHandler;
        readonly IClaimBuilder<RefreshExternal> _refreshClaimBuilder;

        public AuthenticationController(
            IJwtTokenHandler<AccessExternal> accessTokenHandler,
            IClaimBuilder<AccessExternal> accessClaimBuiler,
            IJwtTokenHandler<RefreshExternal> refreshTokenHandler,
            IClaimBuilder<RefreshExternal> refreshClaimBuiler
            )
        {
            _accessTokenHandler = accessTokenHandler;
            _accessClaimBuilder = accessClaimBuiler;
            _refreshTokenHandler = refreshTokenHandler;
            _refreshClaimBuilder = refreshClaimBuiler;
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
