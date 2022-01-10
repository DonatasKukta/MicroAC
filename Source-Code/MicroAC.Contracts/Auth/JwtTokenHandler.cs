using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MicroAC.Core.Auth
{
    public interface IJwtTokenHandler<TokenType>
        where TokenType : IDefaultTokenClaims
    {
        string Create(Dictionary<string, object> claims);
        ClaimsPrincipal Validate(string token);
    }

    public class JwtTokenHandler<TokenType> : IJwtTokenHandler<TokenType>
        where TokenType : IDefaultTokenClaims
    {
        //TODO: Take from config
        TimeSpan _expiration = TimeSpan.FromDays(10);

        readonly SigningCredentials _credentials;
        readonly JwtSecurityTokenHandler _jwtHandler;
        readonly TokenType _token;
        readonly TokenValidationParameters _validationParameters;

        public JwtTokenHandler(TokenType token)
        {
            //TODO: Get key from config
            _credentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.ASCII.GetBytes("Long enough JWT Secret aka Secret Key")),
            SecurityAlgorithms.HmacSha256Signature);
            _jwtHandler = new JwtSecurityTokenHandler();
            _jwtHandler.InboundClaimFilter.Clear();
            _jwtHandler.OutboundClaimTypeMap.Clear();
            _token = token;
            _validationParameters = GetValidationParameters();
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

        //TODO: Additional validation parameters?
        public ClaimsPrincipal Validate(string token) =>
             _jwtHandler.ValidateToken(token, _validationParameters, out SecurityToken validatedToken);
        
        private TokenValidationParameters GetValidationParameters()
        {
            return new TokenValidationParameters()
            {
                ValidateLifetime = true,
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidIssuer = _token.Issuer,
                ValidAudience = _token.Audience,
                IssuerSigningKey = _credentials.Key
            };
        }
    }
}
