using System.Net.Http;

using Microsoft.Extensions.DependencyInjection;

namespace WebShop.Common
{
    public static class BootStrap
    {
        public static void AddWebShopApiClient(this IServiceCollection services)
        {
            var httpClient = new HttpClient();
            var webShopApiClient = new WebShopApiClient(httpClient);

            services.AddSingleton(typeof(IWebShopApiClient), webShopApiClient);
        }
    }
}
