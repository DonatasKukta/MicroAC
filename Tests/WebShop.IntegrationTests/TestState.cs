using System.Net.Http;

namespace WebShop.IntegrationTests
{
    public class TestState
    {
        public HttpResponseMessage Response { get; set; }

        public HttpRequestMessage Request { get; set; }

        public string Url { get; set; }
    }
}
