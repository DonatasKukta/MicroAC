using System;
using System.Collections;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MicroAC.Core.Auth
{
    public struct MicroACClaimTypes
    {
        public const string KeyId = "kid";
        public const string Role = "role";
        public const string SubjectClaims = "sclaims";
        public const string Conditions = "cnd";
    }

    public class ClaimBuilder
    {
        LinkedList<Claim> _claims;

        public ClaimBuilder()
        {
            _claims = new LinkedList<Claim>();
        }

        /// <summary>
        /// Add "Issued Add" claim and flush existing claims.
        /// </summary>
        /// <returns>Collection of added claim types.</returns>
        public IEnumerable<Claim> Build()
        {
            _claims.AddLast(new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToString()));
            var result = _claims;
            _claims = new LinkedList<Claim>();
            return result;
        }

        public ClaimBuilder AddIssuer(string value)
        {
            _claims.AddLast(new Claim(JwtRegisteredClaimNames.Iss, value));
            return this;
        }

        public ClaimBuilder AddSubject(string value)
        {
            _claims.AddLast(new Claim(JwtRegisteredClaimNames.Sub, value));
            return this;
        }

        public ClaimBuilder AddAudience(string value)
        {
            _claims.AddLast(new Claim(JwtRegisteredClaimNames.Aud, value));
            return this;
        }

        public ClaimBuilder AddExpirationTime(string value)
        {
            _claims.AddLast(new Claim(JwtRegisteredClaimNames.Exp, value));
            return this;
        }

        public ClaimBuilder AddJwtId(string value)
        {
            _claims.AddLast(new Claim(JwtRegisteredClaimNames.Jti, value));
            return this;
        }

        public ClaimBuilder AddKeyId(string value)
        {
            _claims.AddLast(new Claim(MicroACClaimTypes.KeyId, value));
            return this;
        }

        public ClaimBuilder AddRole(string value)
        {
            _claims.AddLast(new Claim(MicroACClaimTypes.Role, value));
            return this;
        }

        public ClaimBuilder AddSubjectClaims(string value)
        {
            _claims.AddLast(new Claim(MicroACClaimTypes.SubjectClaims, value));
            return this;
        }

        public ClaimBuilder AddConditions(string value)
        {
            _claims.AddLast(new Claim(MicroACClaimTypes.Conditions, value));
            return this;
        }
    }
}
