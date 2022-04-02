using MicroAC.Core.Common;
using MicroAC.Core.Constants;
using MicroAC.Core.Exceptions;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
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

        StringContent ForwardRequestContent;

        public RequestManagerController(HttpClient httpClient, IConfiguration config)
        {
            _http = httpClient;
            _routes = config.GetSection("EndpointRoutes").Get<List<EndpointRoute>>();
            _basePath = config.GetValue<string>("InternalGateway");
            _authorizationUrl = new Uri(_basePath + config.GetValue<string>("InternalAuthorizationRoute"));
            _headersToIgnore = config.GetSection("HeadersToIgnore").Get<List<string>>();
            _serviceName = config.GetValue<string>("Timestamp:ServiceName");
        }

        public async Task<IActionResult> Index()
        {
            var requestedRoute = GetMatchingEndpointRoute();

            if (requestedRoute is null)
            {
                return NotFound("Requested resource could not be found.");
            }

            if (requestedRoute.RequiresAuhtentication
                && !this.Request.Headers.ContainsKey(HttpHeaders.Authorization))
            {
                throw new AuthenticationFailedException(
                    $"Request path {this.HttpContext.Request.Path} requires external access token.");
            }

            ForwardRequestContent = await GetForwardRequestContent();

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
            var forwardRequest = CreateForwardRequest(forwardUri);

            this.HttpContext.AddActionMessage(_serviceName, "Forward");

            var response = await _http.SendAsync(forwardRequest);

            return await HandleForwardedResponse(response);
        }

        private async Task AuthorizeRequest()
        {
            this.HttpContext.AddActionMessage(_serviceName, "StartAuth");

            var request = CreateForwardRequest(_authorizationUrl);
            request.Method = HttpMethod.Post;
            var response = await _http.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new AuthorizationFailedException("Unable to authorize the request.",
                    new Exception("Response received from Authorization: " + responseContent));
            }

            this.HttpContext.AppendTimestampHeaders(response.Headers);
            this.HttpContext.AddActionMessage(_serviceName, "Authorized");

            this.HttpContext.Request.Headers.Add(HttpHeaders.InternalJWT, responseContent);
        }

        private EndpointRoute GetMatchingEndpointRoute()
        {
            var originalPath = this.HttpContext.Request.Path.ToString();
            return _routes.FirstOrDefault(r => originalPath.StartsWith(r.ExternalRoute));
        }

        private async Task<IActionResult> HandleForwardedResponse(HttpResponseMessage response)
        {
            this.HttpContext.AppendTimestampHeaders(response.Headers);
            
            // TODO: Fix header transfer
            
            this.HttpContext.AddActionMessage(_serviceName, "Receive");

            this.HttpContext.Response.StatusCode = (int)response.StatusCode;

            var body = await response.Content.ReadAsStringAsync();
            
            return new ObjectResult(body);
        }
        
        private HttpRequestMessage CreateForwardRequest(Uri uri)
        {
            var method = new HttpMethod(this.HttpContext.Request.Method);
            var request = new HttpRequestMessage()
            {
                Method = method,
                Content = ForwardRequestContent,
                RequestUri = uri
            };
            
            foreach (var header in this.HttpContext.Request.Headers)
            {
                if (!_headersToIgnore.Contains(header.Key) && !header.Key.StartsWith("Content-"))
                    request.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
            }

            return request;
        }

        private async Task<StringContent> GetForwardRequestContent()
        {
            var requestBody = await new StreamReader(Request.Body, Encoding.Default).ReadToEndAsync();

            if (requestBody == null)
                return null;

            var content = new StringContent(requestBody, Encoding.UTF8, "application/json");
            
            foreach (var header in this.HttpContext.Request.Headers)
            {
                content.Headers.TryAddWithoutValidation(header.Key, header.Value.ToString());
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
