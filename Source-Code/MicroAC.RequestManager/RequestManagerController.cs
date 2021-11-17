using Microsoft.AspNetCore.Mvc;
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
        //TODO: Move to config
        readonly string _basePath = "http://localhost:19083/";

        /// <summary>
        /// List of routes which are used for request forwarding.
        /// TODO: Move to config.
        /// </summary>
        readonly Dictionary<string, string> _routes = new Dictionary<string, string>()
        {
            { "/Authorization", "MicroAC.ServiceFabric/MicroAC.Authorization" },
            { "/Authentication", "MicroAC.ServiceFabric/MicroAC.Authentication" }
        };

        HttpClient _http;

        public RequestManagerController(HttpClient httpClient)
        {
            _http = httpClient;
        }

        public async Task<IActionResult> Index()
        {
            var originalPath = this.HttpContext.Request.Path.ToString();
            var forwardKey = _routes.Keys.FirstOrDefault(key => originalPath.StartsWith(key));

            var bodyStream = new StreamReader(Request.Body);
            var requestBody = await bodyStream.ReadToEndAsync();

            if (forwardKey != null)
            {
                //TODO: Forward content and headers of the request.
                var result =  await _http.PostAsync(_basePath + originalPath.Replace(forwardKey, _routes[forwardKey]), new StringContent(requestBody));
                return Ok(await result.Content.ReadAsStringAsync());
            }

            return NotFound();
        }
    }
}
