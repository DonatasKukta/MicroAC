using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;

namespace MicroAC.Core.Auth
{
    public struct MicroACClaimTypes
    {
        public const string KeyId = "kid";
        public const string UserId = "uid";
        public const string Role = "role";
        public const string SubjectClaims = "sclaims";
        public const string Conditions = "cnd";
    }

    public interface IClaimBuilder<TokenType>
        where TokenType : IDefaultTokenClaims
    {
        Dictionary<string, object> Build();

        IClaimBuilder<TokenType> AddCommonClaims();

        IClaimBuilder<TokenType> AddRole(object value);

        IClaimBuilder<TokenType> AddUserId(object value);

        IClaimBuilder<TokenType> AddSubjectClaims(object value);

        IClaimBuilder<TokenType> AddConditions(object value);
    }

    public class ClaimBuilder<TokenType> : IClaimBuilder<TokenType>
        where TokenType : IDefaultTokenClaims
    {
        Dictionary<string, object> _defaultClaims;
        Dictionary<string, object> _claims;
        readonly TokenType _token;

        public ClaimBuilder(TokenType token)
        {
            _token = token;
            _claims = new Dictionary<string, object>(5);
            SetDefaultClaims();
        }

        /// <summary>
        /// Flush existing claims.
        /// </summary>
        /// <returns> Collection of added claim types. </returns>
        public Dictionary<string, object> Build()
        {
            var result = _claims;
            _claims = new Dictionary<string, object>(5);
            return result;
        }

        public IClaimBuilder<TokenType> AddCommonClaims()
        {
            foreach (var defaultClaim in _defaultClaims)
                _claims.Add(defaultClaim.Key, defaultClaim.Value);
            return this;
        }

        public IClaimBuilder<TokenType> AddRole(object value)
        {
            _claims.Add(MicroACClaimTypes.Role, value);
            return this;
        }

        public IClaimBuilder<TokenType> AddSubjectClaims(object value)
        {
            _claims.Add(MicroACClaimTypes.SubjectClaims, value);
            return this;
        }

        public IClaimBuilder<TokenType> AddUserId(object value)
        {
            _claims.Add(MicroACClaimTypes.UserId, value);
            return this;
        }

        public IClaimBuilder<TokenType> AddConditions(object value)
        {
            _claims.Add(MicroACClaimTypes.Conditions, value);
            return this;
        }

        private IClaimBuilder<TokenType> AddIssuer(object value)
        {
            _claims.Add(JwtRegisteredClaimNames.Iss, value);
            return this;
        }

        private IClaimBuilder<TokenType> AddSubject(object value)
        {
            _claims.Add(JwtRegisteredClaimNames.Sub, value);
            return this;
        }

        private IClaimBuilder<TokenType> AddAudience(object value)
        {
            _claims.Add(JwtRegisteredClaimNames.Aud, value);
            return this;
        }

        private IClaimBuilder<TokenType> AddJwtId(object value)
        {
            _claims.Add(JwtRegisteredClaimNames.Jti, value);
            return this;
        }

        public Dictionary<string, object> GetDefaulClaims()
        {
            return _defaultClaims;
        }

        private void SetDefaultClaims()
        {
            var temp = _claims;
            _claims = new Dictionary<string, object>();

            AddJwtId(Guid.NewGuid().ToString());
            AddIssuer(_token.Issuer);
            AddAudience(_token.Audience);
            AddSubject(_token.Subject);
            _defaultClaims = _claims;
            _claims = temp;
        }
    }
}
