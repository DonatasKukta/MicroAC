using System.Net.Http;
using System.Threading.Tasks;

namespace WebShop.Common
{
    public interface IWebShopApiClient
    {
        Task<HttpResponseMessage> SendServiceRequest(
            WebShopServices service,
            HttpMethod method,
            string route,
            string authToken = "",
            object body = null);
    }
}
