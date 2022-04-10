using System.Net.Http;
using System.Threading.Tasks;

using MicroAC.Core.Client;

using Microsoft.AspNetCore.Http;

namespace WebShop.Common
{
    public interface IWebShopApiClient
    {
        Task<HttpResponseMessage> SendServiceRequest(
            HttpContext context,
            MicroACServices service,
            HttpMethod method,
            string route = "/",
            object body = null);
    }
}
