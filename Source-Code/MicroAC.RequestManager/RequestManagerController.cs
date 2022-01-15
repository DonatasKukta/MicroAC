using Microsoft.AspNetCore.Mvc;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace MicroAC.RequestManager
{
    [Route("/{*url}")]
    public class RequestManagerController : Controller
    {
        readonly Uri _authorizationUrl;

        readonly string _basePath = "http://localhost:19083/";
        
        readonly HttpClient _http;

        /// <summary>
        /// List of routes which are used for request forwarding.
        /// </summary>
        //TODO: Read from config.
        readonly List<EndpointRoute> _routesNew = new List<EndpointRoute>
        {
            new EndpointRoute {
                ExternalRoute = "/Authentication",
                InternalRoute = "MicroAC.ServiceFabric/MicroAC.Authentication",
                RequiresAuhtentication = false,
                RequiresAuthorization = false
            },
            new EndpointRoute {
                ExternalRoute = "/ResourceApi",
                InternalRoute = "MicroAC.ServiceFabric/Example.ResourceApi",
                RequiresAuhtentication = true,
                RequiresAuthorization = true
            },
        };

        readonly List<string> _headersToIgnore = new List<string>()
        {
            "Host",
            "User-Agent",
            "Cache-Control",
            "Content-Length"
        };

        public RequestManagerController(HttpClient httpClient)
        {
            _http = httpClient;
            _authorizationUrl = new Uri(_basePath + "MicroAC.ServiceFabric/MicroAC.Authorization/Authorize");
        }

        public async Task<IActionResult> Index()
        {
            var requestedRoute = GetMatchingEndpointRoute();

            if(requestedRoute is null) 
                return NotFound("Requested resource could not be found.");

            if (requestedRoute.RequiresAuhtentication)
            {
                var containsExternalAccessToken = this.Request.Headers.ContainsKey("Authorization");
                if (!containsExternalAccessToken)
                {
                    return Unauthorized("Access token is missing.");
                }
            }

            if (requestedRoute.RequiresAuthorization)
            {
                var authorised = await AuthorizeRequest();

                if (!authorised)
                {
                    return Unauthorized("Unable to authorize the request.");
                }
            }

            var forwardUri = GetForwardUri(requestedRoute);
            var forwardRequest = await CreateForwardRequest(forwardUri);
            var response = await _http.SendAsync(forwardRequest);

            return await HandleForwardedResponse(response);
        }

        private async Task<bool> AuthorizeRequest()
        {
             var request = await CreateForwardRequest(_authorizationUrl);
            request.Method = HttpMethod.Post;
            var result = await _http.SendAsync(request);

            if (!result.IsSuccessStatusCode)
            {
                return false;
            }

            var token = await result.Content.ReadAsStringAsync();
            this.HttpContext.Request.Headers.Add("MicroAC-JWT", token);

            return true;
        }

        private EndpointRoute GetMatchingEndpointRoute()
        {
            var originalPath = this.HttpContext.Request.Path.ToString();
            return _routesNew.FirstOrDefault(r => originalPath.StartsWith(r.ExternalRoute));
        }

        private async Task<IActionResult> HandleForwardedResponse(HttpResponseMessage response)
        {
            foreach (var header in Request.Headers)
            {
                if (!_headersToIgnore.Contains(header.Key))
                    this.HttpContext.Response.Headers.Add(header.Key, header.Value.ToString());
            }

            this.HttpContext.Response.StatusCode = (int)response.StatusCode;

            return new ObjectResult(await response.Content.ReadAsStringAsync());
        }

        /// <summary>
        /// Creates HttpRequestMessage from HttpContext. Body string, headers and HTTP method are transfered,
        /// </summary>
        private async Task<HttpRequestMessage> CreateForwardRequest(Uri uri)
        {
            var request = new HttpRequestMessage()
            {
                Method = new HttpMethod(this.HttpContext.Request.Method),
                Content = await GetForwardRequestContent(),
                RequestUri = uri
            };

            foreach (var header in this.HttpContext.Request.Headers)
            {
                if (!_headersToIgnore.Contains(header.Key) && !header.Key.StartsWith("Content-"))
                    request.Headers.Add(header.Key, header.Value.ToString());
            }

            return request;
        }

        private async Task<StringContent> GetForwardRequestContent()
        {
            var bodyStream = new StreamReader(this.Request.Body);
            var requestBody = await bodyStream.ReadToEndAsync();
            var content = new StringContent(requestBody);

            foreach (var header in this.HttpContext.Request.Headers)
            {
                if (!_headersToIgnore.Contains(header.Key) && header.Key.StartsWith("Content-"))
                {
                    content.Headers.Remove(header.Key);
                    content.Headers.Add(header.Key, header.Value.ToString());
                }
            }

            return content;
        }

        private Uri GetForwardUri(EndpointRoute requestedRoute)
        {
            var originalPath = this.HttpContext.Request.Path.ToString();
            var forwardUri = _basePath
                + originalPath.Replace(requestedRoute.ExternalRoute, requestedRoute.InternalRoute)
                + this.Request.QueryString.ToUriComponent();

            return new Uri(forwardUri);
        }
    }
}
