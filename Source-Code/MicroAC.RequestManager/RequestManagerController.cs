using MicroAC.Core.Common;
using MicroAC.Core.Exceptions;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MicroAC.RequestManager
{
    [Route("/{*url}")]
    public class RequestManagerController : Controller
    {
        readonly Uri _authorizationUrl;

        readonly string _basePath;

        readonly HttpClient _http;

        readonly List<EndpointRoute> _routes;

        readonly List<string> _headersToIgnore;

        readonly string _serviceName;

        readonly string _timestampHeader;

        public RequestManagerController(HttpClient httpClient, IConfiguration config)
        {
            _http = httpClient;
            _routes = config.GetSection("EndpointRoutes").Get<List<EndpointRoute>>();
            _basePath = config.GetValue<string>("InternalGateway");
            _authorizationUrl = new Uri(_basePath + config.GetValue<string>("InternalAuthorizationRoute"));
            _headersToIgnore = config.GetSection("HeadersToIgnore").Get<List<string>>();
            _serviceName = config.GetValue<string>("Timestamp:ServiceName");
            _timestampHeader = config.GetValue<string>("Timestamp:Header");
        }

        public async Task<IActionResult> Index()
        {
            var requestedRoute = GetMatchingEndpointRoute();

            if (requestedRoute is null)
            {
                return NotFound("Requested resource could not be found.");
            }

            if (requestedRoute.RequiresAuhtentication
                && !this.Request.Headers.ContainsKey("Authorization"))
            {
                throw new AuthenticationFailedException(
                    $"Request path {this.HttpContext.Request.Path} requires external access token.");
            }

            if (requestedRoute.RequiresAuthorization)
            {
                await AuthorizeRequest();
            }

            var response = await ForwardRequest(requestedRoute);
            return response;
        }

        private async Task<IActionResult> ForwardRequest(EndpointRoute requestedRoute)
        {
            var forwardUri = GetForwardUri(requestedRoute);
            var forwardRequest = await CreateForwardRequest(forwardUri);

            this.HttpContext.AddActionMessage(_timestampHeader, _serviceName, "Forward");

            var response = await _http.SendAsync(forwardRequest);

            return await HandleForwardedResponse(response);
        }

        private async Task AuthorizeRequest()
        {
            this.HttpContext.AddActionMessage(_timestampHeader, _serviceName, "StartAuth");

            var request = await CreateForwardRequest(_authorizationUrl);
            request.Method = HttpMethod.Post;
            var response = await _http.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new AuthorizationFailedException("Unable to authorize the request.",
                    new Exception("Response received from Authorization: " + responseContent));
            }

            this.HttpContext.AppendeTimestampHeaders(_timestampHeader, response.Headers);
            this.HttpContext.AddActionMessage(_timestampHeader, _serviceName, "Authorized");

            this.HttpContext.Request.Headers.Add("MicroAC-JWT", responseContent);
        }

        private EndpointRoute GetMatchingEndpointRoute()
        {
            var originalPath = this.HttpContext.Request.Path.ToString();
            return _routes.FirstOrDefault(r => originalPath.StartsWith(r.ExternalRoute));
        }

        private async Task<IActionResult> HandleForwardedResponse(HttpResponseMessage response)
        {
            this.HttpContext.AppendeTimestampHeaders(_timestampHeader, response.Headers);
            /* TODO: Fix header transfer
            foreach (var header in response.Headers)
            {
                if (!_headersToIgnore.Contains(header.Key) && header.Key != _timestampHeader)
                {
                    this.HttpContext.Response.Headers.Add(header.Key, header.Value.ToString());
                }
            }
            */
            this.HttpContext.AddActionMessage(_timestampHeader, _serviceName, "Receive");

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
