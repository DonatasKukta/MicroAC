using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

using MicroAC.Core.Common;
using MicroAC.Core.Constants;
using MicroAC.Core.Client;
using Microsoft.Extensions.Configuration;

namespace WebShop.Common
{
    public class WebShopApiClient : IWebShopApiClient
    {
        readonly HttpClient HttpClient;

        readonly IEndpointResolver EndpointResolver;

        readonly bool IsCentralAuthorizationEnabled;

        public WebShopApiClient(
            HttpClient httpClient,
            IEndpointResolver endpointResolver,
            IConfiguration configuration)
        {
            HttpClient = httpClient;
            EndpointResolver = endpointResolver;
            IsCentralAuthorizationEnabled = configuration.GetValue<bool>(ConfigKeys.CentralAuthorizationEnabled);
        }

        public async Task<HttpResponseMessage> SendServiceRequest(
            HttpContext context,
            MicroACServices service,
            HttpMethod method,
            string route,
            object body = null)
        {
            var request = new HttpRequestMessage
            {
                RequestUri = await GetServiceUrl(service, route),
                Method = method,
            };

            var authHeader = IsCentralAuthorizationEnabled
                ? HttpHeaders.InternalJWT     
                : HttpHeaders.Authorization;    

            var headerValue = context.Request.Headers.GetCommaSeparatedValues(authHeader);
            request.Headers.Add(authHeader, headerValue);

            if (body != null)
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

        async Task<Uri> GetServiceUrl(MicroACServices service, string route)
        {
            var endpoint = await EndpointResolver.GetServiceEndpoint(service);

            return new Uri(endpoint + route);
        }
    }
}
