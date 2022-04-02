using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

using MicroAC.Core.Common;
using MicroAC.Core.Constants;
using MicroAC.Core.Client;

namespace WebShop.Common
{
    public class WebShopApiClient : IWebShopApiClient
    {        
        readonly HttpClient HttpClient;

        readonly IEndpointResolver EndpointResolver;

        public WebShopApiClient(HttpClient httpClient, IEndpointResolver endpointResolver)
        {
            HttpClient = httpClient;
            EndpointResolver = endpointResolver;
        }

        public async Task<HttpResponseMessage> SendServiceRequest(
            HttpContext context,
            MicroACServices service,
            HttpMethod method,
            string route,
            string authToken = "",
            object body = null)
        {
            var request = new HttpRequestMessage
            {
                RequestUri = await GetServiceUrl(service, route),
                Method = method,
            };

            if (authToken != "")
                request.Headers.Add(HttpHeaders.Authorization, authToken);
            
            if(body != null)
                request.Content = new StringContent(
                    JsonSerializer.Serialize(body),
                    Encoding.UTF8,
                    "application/json"
                    );

            var response = await HttpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            context.AppendTimestampHeaders(response.Headers);

            return response;
        }

        public async Task<Uri> GetServiceUrl(MicroACServices service, string route)
        {
            var endpoint = await EndpointResolver.GetServiceEndpoint(service);
            
            return new Uri(endpoint + route);
        }
    }
}
