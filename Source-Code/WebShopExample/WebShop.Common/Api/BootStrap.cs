using System.Net.Http;

using MicroAC.Core.Auth;
using MicroAC.Core.Client;
using MicroAC.Core.Constants;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace WebShop.Common
{
    public static class BootStrap
    {
        public static void AddWebShopServiceDependencies(this IServiceCollection services)
        {
            services.AddSingleton<AccessInternal>();

            services.AddScoped<IJwtTokenHandler<AccessInternal>, JwtTokenHandler<AccessInternal>>();

            services.AddSingleton<IAuthorizationServiceClient, AuthorizationServiceClient>();

            services.AddSingleton<HttpClient>();

            services.AddSingleton<IEndpointResolver, FabricReverseProxyEndpointResolver>();
            //services.AddSingleton<IEndpointResolver, FabricEndpointResolver>();
            
            services.AddSingleton<IWebShopApiClient, WebShopApiClient>();
        }
    }
}
