﻿using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace WebShop.IntegrationTests
{
    public class TestState
    {
        public HttpResponseMessage Response { get; set; }

        public HttpRequestMessage Request { get; set; }
    }
}
