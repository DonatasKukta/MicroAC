using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

namespace WebShop.Common
{
    public interface IWebShopApiClient
    {
        Task<HttpResponseMessage> SendServiceRequest(
            HttpContext context,
            WebShopServices service,
            HttpMethod method,
            string route,
            string authToken = "",
            object body = null);
    }
}
