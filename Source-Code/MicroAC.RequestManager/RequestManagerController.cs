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
        //TODO: Move to config
        readonly string _basePath = "http://localhost:19083/";

        HttpClient _http;

        /// <summary>
        /// List of routes which are used for request forwarding.
        /// TODO: Read from config.
        /// </summary>
        readonly Dictionary<string, string> _routes = new Dictionary<string, string>()
        {
            { "/Authorization", "MicroAC.ServiceFabric/MicroAC.Authorization" },
            { "/Authentication", "MicroAC.ServiceFabric/MicroAC.Authentication" }
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
        }

        public async Task<IActionResult> Index()
        {
            var forwardRequest = await CreateForwardRequest();
            var response = await _http.SendAsync(forwardRequest);
            
            return await HandleForwardedResponse(response);
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
        private async Task<HttpRequestMessage> CreateForwardRequest()
        {
            var request = new HttpRequestMessage()
            {
                Method = new HttpMethod(this.HttpContext.Request.Method),
                Content = await GetForwardRequestContent(),
                RequestUri = GetForwardUri()
            };

            foreach (var header in this.HttpContext.Request.Headers)
            {
                if(!_headersToIgnore.Contains(header.Key) && !header.Key.StartsWith("Content-"))
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

        private Uri GetForwardUri()
        {
            var originalPath = this.HttpContext.Request.Path.ToString();
            var forwardKey = _routes.Keys.FirstOrDefault(key => originalPath.StartsWith(key));
            var forwardUri = _basePath 
                + originalPath.Replace(forwardKey, _routes[forwardKey]) 
                + this.Request.QueryString.ToUriComponent();

            return new Uri(forwardUri);
        }
    }
}
