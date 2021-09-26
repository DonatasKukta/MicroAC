using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace MicroAC.Core.Auth
{
    public enum TokenType
    {
        AccessExternal,
        RefreshExternal,
        AccessInternal,
    }

    public struct MicroACClaimTypes
    {
        public const string KeyId = "kid";
        public const string Role = "role";
        public const string SubjectClaims = "sclaims";
        public const string Conditions = "cnd";
    }

    public class ClaimBuilder
    {
        Dictionary<string, object> _defaultClaims;
        Dictionary<string, object> _claims;
        readonly TokenType _type;
        readonly double _expirationSeconds;

        public ClaimBuilder(TokenType type, double expirationSeconds)
        {
            _type = type;
            _expirationSeconds = expirationSeconds;
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

        public ClaimBuilder AddCommonClaims()
        {
            foreach (var defaultClaim in _defaultClaims)
                _claims.Add(defaultClaim.Key, defaultClaim.Value);
            return this;
        }

        public ClaimBuilder AddRole(object value)
        {
            _claims.Add(MicroACClaimTypes.Role, value);
            return this;
        }

        public ClaimBuilder AddSubjectClaims(object value)
        {
            _claims.Add(MicroACClaimTypes.SubjectClaims, value);
            return this;
        }

        public ClaimBuilder AddConditions(object value)
        {
            _claims.Add(MicroACClaimTypes.Conditions, value);
            return this;
        }

        private ClaimBuilder AddIssuer(object value)
        {
            _claims.Add(JwtRegisteredClaimNames.Iss, value);
            return this;
        }

        private ClaimBuilder AddSubject(object value)
        {
            _claims.Add(JwtRegisteredClaimNames.Sub, value);
            return this;
        }

        private ClaimBuilder AddAudience(object value)
        {
            _claims.Add(JwtRegisteredClaimNames.Aud, value);
            return this;
        }

        private ClaimBuilder AddJwtId(object value)
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
            switch (_type)
            {
                //TODO: Move hardcoded strings to constants.
                case TokenType.AccessExternal:
                case TokenType.RefreshExternal:
                    AddIssuer("MicroAC:AuthenticationService");
                    AddAudience("MicroAC:AuthorizationService");
                    AddSubject("MicroAC:User");
                    break;
                case TokenType.AccessInternal:
                    AddIssuer("MicroAC:AuthorizationService");
                    AddAudience("MicroAC:Services");
                    AddSubject("MicroAC:Request");
                    break;
            }
            _defaultClaims = _claims;
            _claims = temp;
        }
    }
}
