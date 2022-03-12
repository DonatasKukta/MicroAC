using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using FluentAssertions;

using TechTalk.SpecFlow;

namespace WebShop.IntegrationTests.Steps
{
    public class SharedSteps
    {
        protected readonly Uri Url;

        protected readonly TestState State;

        readonly HttpClient HtppClient = new HttpClient();

        public SharedSteps(Uri baseUri)
        {
            Url = baseUri;
            State = new TestState
            {
                Request = new HttpRequestMessage()
                {
                    RequestUri = baseUri
                },
                Response = new HttpResponseMessage()
            };
        }

        [Given(@"(.*) request")]
        public void GivenGetProductsRequest(string method)
        {
            State.Request.Method = new HttpMethod(method);
        }

        [When(@"request is sent")]
        public async Task WhenRequestIsSent()
        {
            State.Response = await HtppClient.SendAsync(State.Request);
        }

        [Then(@"response status code is (.*)")]
        public void ThenResponseStatusCodeIs(int status)
        {
            State.Response.StatusCode.Should().Be((HttpStatusCode)status);
        }

        protected void AppendToRequestUrl(string uri)
        {
            State.Request.RequestUri =
                new Uri($"{State.Request.RequestUri}/{uri}");
        }

        protected void SetJsonBody(object body)
        {
            var productStr = JsonSerializer.Serialize(body);
            State.Request.Content = new StringContent(
                productStr,
                Encoding.UTF8,
                "application/json"
                );
        }

        protected async Task<T> GetResponseJsonData<T>()
        {
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = false
            };
            var str = await State.Response.Content.ReadAsStringAsync();
            var obj = JsonSerializer.Deserialize<T>(str, options);
            return obj;
        }
    }
}
