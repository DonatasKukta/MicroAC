using System.Collections.Generic;
using System.Linq;

using MicroAC.Core.Client;

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

        public const string FabricRequestId = "X-ServiceFabricRequestId";
    }

    public static class ConfigKeys
    {
        public const string CentralAuthorizationEnabled = "CentralAuthorizationEnabled";

        public const string SfClusterClientConnectionEndpoint = "SfClusterClientConnectionEndpoint";
        
        public const string SfReverseProxyPorts = "SfReverseProxyPorts";

        public const string SfReverseProxyIp = "SfReverseProxyIp";

        public const string SfReverseProxyEnabled = "SfReverseProxyEnabled";

        public const string StrictAuthorizationEnabled = "StrictAuthorizationEnabled";
    }

    public static class HttpContextKeys
    {
        public const string Permissions = "ValidatedPermissions";

        public const string InternalAccessToken = "ValidatedInternalAccessToken";

        public const string Roles = "ValidatedRoles";
    }

    internal static class Fabric
    {
        static readonly Dictionary<MicroACServices, string> FabricServices = new()
        {
            { MicroACServices.ResourceApi,    "Example.ResourceApi" },
            { MicroACServices.RequestManager, "MicroAC.RequestManager" },
            { MicroACServices.Authentication, "MicroAC.Authentication" },
            { MicroACServices.Authorization,  "MicroAC.Authorization" },
            { MicroACServices.Orders,         "WebShop.Orders" },
            { MicroACServices.Shipments,      "WebShop.Shipments" },
            { MicroACServices.Cart,           "WebShop.Cart" },
            { MicroACServices.Products,       "WebShop.Products" }
        };

        public static string GetServiceTypeName(MicroACServices service) => FabricServices[service];

        public static IEnumerable<MicroACServices> GetRegisteredServices() 
            => FabricServices.Keys.ToList();
    }

}
