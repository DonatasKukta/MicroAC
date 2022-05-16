using System;
using System.Threading.Tasks;

using FluentAssertions;

using MicroAC.Core.Client;

using TechTalk.SpecFlow;

using WebShop.Common;
using Microsoft.Extensions.Configuration;
using MicroAC.Core.Models;
using MicroAC.Core.Auth;
using System.Net.Http;

namespace WebShop.IntegrationTests.Steps
{
    [Binding]
    [Scope(Feature = "Authentication")]
    class AuthenticationSteps : SharedSteps
    {
        ResponseTokens Response = null;

        public AuthenticationSteps() : base(MicroACServices.Authentication)
        {

        }

        [Given(@"login path")]
        public void GivenLoginPath()
        {
            this.AppendToRequestUrl("Login");
        }

        [Given(@"test user credentials")]
        public void GivenTestUserCredentials()
        {
            var credentials = new LoginCredentials
            {
                Email = this.Configuration.GetValue<string>("TestUsername"),
                Password = this.Configuration.GetValue<string>("TestPassword")
            };
            this.SetJsonBody(credentials);
        }

        [Given(@"refresh path")]
        public void GivenRefreshPath()
        {
            this.AppendToRequestUrl("Refresh");
        }

        [Given(@"refresh token")]
        public void GivenRefreshToken()
        {
            var resfreshToken = this.Configuration.GetValue<string>("TestRefreshToken");

            State.Request.Content = new StringContent(resfreshToken);
        }

        [Then(@"response contains access and refresh tokens")]
        public async Task ThenResponseContainsAccessAndRefreshTokens()
        {
            Response = await GetResponseJsonData<ResponseTokens>();

            Response.Should().NotBeNull();
            Response.AccessJwt.Should().NotBeNullOrWhiteSpace();
            Response.RefreshJwt.Should().NotBeNullOrWhiteSpace();
        }

        [Then(@"access token is valid")]
        public void ThenAccessTokenIsValid()
        {
            var accessTokenHandler = new JwtTokenHandler<AccessExternal>(new AccessExternal());
            var claims = accessTokenHandler.Validate(Response.AccessJwt);

            claims.Should().NotBeNull();
        }

        [Then(@"refresh token is valid")]
        public void ThenRefreshTokenIsValid()
        {
            var refreshTokenHandler = new JwtTokenHandler<RefreshExternal>(new RefreshExternal());
            var claims = refreshTokenHandler.GetValidatedPermissions(Response.RefreshJwt);

            claims.Should().NotBeNull();
        }

        [Then(@"response contains access token")]
        public async Task ThenResponseContainsAccessToken()
        {
            var accessToken = await State.Response.Content.ReadAsStringAsync();
            this.Response = new ResponseTokens
            {
                AccessJwt = accessToken
            };

            accessToken.Should().NotBeNullOrWhiteSpace();
        }

        class ResponseTokens
        {
            public string AccessJwt { get; set; }
            public string RefreshJwt { get; set; }
        }
    }
}
