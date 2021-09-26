using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;

namespace MicroAC.Core.Auth
{
    public class JwtTokenHandler
    {
        SigningCredentials _credentials;
        JwtSecurityTokenHandler _jwtHandler;
        TimeSpan _expiration = TimeSpan.FromDays(10);

        public JwtTokenHandler()
        {
            _credentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.ASCII.GetBytes("Long enough JWT Secret aka Secret Key")),
            SecurityAlgorithms.HmacSha256Signature);

            _jwtHandler = new JwtSecurityTokenHandler();
        }

        public string Create(Dictionary<string, object> claims)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Claims = claims,
                Expires = DateTime.UtcNow.Add(_expiration),
                IssuedAt = DateTime.UtcNow,
                SigningCredentials = _credentials
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public SecurityToken Validate(string token)
        {
            var validationParameters = GetValidationParameters();
            SecurityToken validatedToken;
            IPrincipal principal = _jwtHandler.ValidateToken(token, validationParameters, out validatedToken);
            return validatedToken;
        }

        public static long GetUnixTime(DateTime date)
        {
            return ((DateTimeOffset)date).ToUnixTimeSeconds();
        }

        private TokenValidationParameters GetValidationParameters()
        {
            return new TokenValidationParameters()
            {
                ValidateLifetime = true,
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidIssuer = "MicroAC:AuthenticationService",
                ValidAudience = "MicroAC:AuthorizationService",
                IssuerSigningKey = _credentials.Key // The same key as the one that generate the token
            };
        }
    }
}
