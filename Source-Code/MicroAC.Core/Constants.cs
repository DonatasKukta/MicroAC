namespace MicroAC.Core.Constants
{
    public static class HttpHeaders
    {
        /// <summary>
        /// Header which contains token issued by Authentication Service 
        /// </summary>
        public const string Authorization = "Authorization";

        /// <summary>
        /// Header which contains token issued by Authentication Service
        /// </summary>
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
