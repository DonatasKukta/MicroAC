﻿using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MicroAC.Core.Auth
{
    public class JwtTokenHandler
    {
        //TODO: Take from config
        TimeSpan _expiration = TimeSpan.FromDays(10);

        readonly SigningCredentials _credentials;
        readonly JwtSecurityTokenHandler _jwtHandler;
        readonly IDefaultTokenClaims _token;
        readonly TokenValidationParameters _validationParameters;

        public JwtTokenHandler(IDefaultTokenClaims token)
        {
            _credentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.ASCII.GetBytes("Long enough JWT Secret aka Secret Key")),
            SecurityAlgorithms.HmacSha256Signature);
            _jwtHandler = new JwtSecurityTokenHandler();
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
