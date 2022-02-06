using MicroAC.Core.Models;

using Microsoft.IdentityModel.Tokens;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace MicroAC.Core.Auth
{
    public interface IJwtTokenHandler<TokenType>
        where TokenType : IDefaultTokenClaims
    {
        string Create(Dictionary<string, object> claims);

        ClaimsPrincipal Validate(string token);

        IEnumerable<Permission> GetValidatedPermissions(string token);
    }

    public class JwtTokenHandler<TokenType> : IJwtTokenHandler<TokenType>
        where TokenType : IDefaultTokenClaims
    {
        //TODO: Take from config
        readonly TimeSpan _expiration = TimeSpan.FromDays(200);

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

        public ClaimsPrincipal Validate(string token) =>
             _jwtHandler.ValidateToken(token, _validationParameters, out _);

        public IEnumerable<Permission> GetValidatedPermissions(string token)
        {
            var claimsPrincipal = _jwtHandler.ValidateToken(token, _validationParameters, out var stoken);

            var sClaims = claimsPrincipal.Claims.Where(c => c.Type == MicroACClaimTypes.SubjectClaims);
            var result = sClaims.Select(c => JsonSerializer.Deserialize<Permission>(c.Value));

            return result;
        }

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
