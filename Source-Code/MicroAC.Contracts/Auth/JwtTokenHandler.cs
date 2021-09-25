using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MicroAC.Core.Auth
{
    public class JwtTokenHandler
    {
        SigningCredentials _credentials;
        JwtSecurityTokenHandler _jwtHandler;

        public JwtTokenHandler()
        {
            _credentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.ASCII.GetBytes("Long enough JWT Secret aka Secret Key")),
            SecurityAlgorithms.HmacSha256Signature);

            _jwtHandler = new JwtSecurityTokenHandler();
        }

        public string Create(IEnumerable<Claim> claims)
        {
            var token = new JwtSecurityToken
            (
                claims: claims,
                signingCredentials: _credentials
            );

            return _jwtHandler.WriteToken(token);
        }

        public bool Validate(string token)
        {
            return _jwtHandler.CanReadToken(token);
        }
    }
}
