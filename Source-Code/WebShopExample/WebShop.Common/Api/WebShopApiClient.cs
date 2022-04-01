using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

using MicroAC.Core.Common;

namespace WebShop.Common
{
    public enum WebShopServices
    {
        Orders,
        Shipments, 
        Cart,
        Products,
    }

    public class WebShopApiClient : IWebShopApiClient
    {
        static readonly string TimestampHeader = "MicroAC-Timestamp";

        static readonly string BaseUrl = "http://localhost:19081/MicroAC.ServiceFabric/WebShop.";
        
        readonly HttpClient HttpClient;

        public WebShopApiClient(HttpClient httpClient)
        {
            HttpClient = httpClient;
        }

        public async Task<HttpResponseMessage> SendServiceRequest(
            HttpContext context,
            WebShopServices service,
            HttpMethod method,
            string route,
            string authToken = "",
            object body = null)
        {
            var request = new HttpRequestMessage
            {
                RequestUri = GetServiceUrl(service, route),
                Method = method,
            };

            if (authToken != "")
                request.Headers.Add("Authorization", authToken);
            
            if(body != null)
                request.Content = new StringContent(
                    JsonSerializer.Serialize(body),
                    Encoding.UTF8,
                    "application/json"
                    );

            var response = await HttpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            context.AppendTimestampHeaders(TimestampHeader, response.Headers);

            return response;
        }

        public Uri GetServiceUrl(WebShopServices service, string route)
        {
            var serviceStr = Enum.GetName(typeof(WebShopServices), service);
            
            return new Uri(BaseUrl + serviceStr + route);
        }
    }
}
