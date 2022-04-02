using System.Net.Http;

using MicroAC.Core.Client;

using Microsoft.Extensions.DependencyInjection;

namespace WebShop.Common
{
    public static class BootStrap
    {
        public static void AddWebShopApiClient(this IServiceCollection services)
        {
            services.AddSingleton< HttpClient>();
            
            services.AddSingleton<IEndpointResolver,FabricEndpointResolver>();
            
            services.AddSingleton<IWebShopApiClient, WebShopApiClient>();
        }
    }
}
