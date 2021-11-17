namespace MicroAC.Core.Auth
{
    public enum TokenType
    {
        AccessExternal,
        RefreshExternal,
        AccessInternal,
    }

    public interface IDefaultTokenClaims
    {
        string Issuer { get; }
        string Audience { get; }
        string Subject { get; }
        TokenType Type { get; }
    }

    public class AccessInternal : IDefaultTokenClaims
    {
        public string Issuer => "MicroAC:AuthorizationService";
        public string Audience => "MicroAC:Service";
        public string Subject => "MicroAC:Request";
        public TokenType Type => TokenType.AccessInternal;
    }

    public class AccessExternal : IDefaultTokenClaims
    {
        public string Issuer => "MicroAC:AuthenticationService";
        public string Audience => "MicroAC:AuthorizationService";
        public string Subject => "MicroAC:User";
        public TokenType Type => TokenType.AccessExternal;
    }

    public class RefreshExternal : IDefaultTokenClaims
    {
        public string Issuer => "MicroAC:AuthenticationService";
        public string Audience => "MicroAC:AuthenticationService";
        public string Subject => "MicroAC:User";
        public TokenType Type => TokenType.RefreshExternal;
    }
}
