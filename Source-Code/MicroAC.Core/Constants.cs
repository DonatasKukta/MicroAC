namespace MicroAC.Core.Constants
{
    public static class HttpHeaders
    {
        public const string Authorization = "Authorization";

        public const string InternalJWT = "MicroAC-JWT";

        public const string Timestamps = "MicroAC-Timestamp";
    }

    public static class ConfigKeys
    {
        public const string CentralAuthorizationEnabled = "CentralAuthorizationEnabled";

        public const string StrictAuthorizationEnabled = "StrictAuthorizationEnabled";
    }

    public static class HttpContextKeys
    {
        public const string Permissions = "Permissions";
    }
}
