using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MicroAC.RequestManager
{
    [Route("/{*url}")]
    public class RequestManagerController : Controller
    {
        HttpClient _http;

        public RequestManagerController(HttpClient httpClient)
        {
            _http = httpClient;
        }

        public async Task<IActionResult> Index()
        {
            var result = await _http.SendAsync(new HttpRequestMessage(HttpMethod.Get, "http://localhost:19081/MicroAC.ServiceFabric/MicroAC.Authorization/"));
            
            return Ok(await result.Content.ReadAsStringAsync());
        }
    }
}
