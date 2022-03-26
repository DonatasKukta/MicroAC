using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using FluentAssertions;

using TechTalk.SpecFlow;

using WebShop.Common;

namespace WebShop.IntegrationTests.Steps
{
    public class SharedSteps
    {
        //TODO: Move to config
        static readonly string TestAuthToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiI0YzlkZjM3MS1lZjVlLTRhOTgtYTllNC00ODUyNDA3ZDI0MGEiLCJpc3MiOiJNaWNyb0FDOkF1dGhlbnRpY2F0aW9uU2VydmljZSIsImF1ZCI6Ik1pY3JvQUM6QXV0aG9yaXphdGlvblNlcnZpY2UiLCJzdWIiOiJNaWNyb0FDOlVzZXIiLCJ1aWQiOiJmZTk4MzEwYi1lYmMyLTQyOTktOTg1NC1mYmVhNDZmNjI1OTEiLCJ1cm9sZXMiOlsiUm9sZTEwX1NlZWRUZXN0RGF0YSJdLCJuYmYiOjE2NDc3OTk4MDQsImV4cCI6MTY2NTA3OTgwNCwiaWF0IjoxNjQ3Nzk5ODA0fQ.zeEV6S0dbzNZMwTBxadf34WJzXPjTmFBzx7s_EAOCVA";

        static readonly string BaseUrl = "http://localhost:19081/MicroAC.ServiceFabric/MicroAC.RequestManager/";

        protected readonly Uri Url;

        protected readonly TestState State;

        protected readonly DataGenerator RequestDataGenerator;

        readonly HttpClient HttpClient;

        public SharedSteps(string serviceUri)
        {
            Url = new Uri(BaseUrl + serviceUri);
            HttpClient = new HttpClient();
            RequestDataGenerator = new DataGenerator();
            State = new TestState
            {
                Request = new HttpRequestMessage()
                {
                    RequestUri = Url
                },
                Response = new HttpResponseMessage()
            };
            State.Request.Headers.Add("Authorization", TestAuthToken);
        }

        [Given(@"(.*) request")]
        public void GivenGetProductsRequest(string method)
        {
            State.Request.Method = new HttpMethod(method);
        }

        [When(@"request is sent")]
        public async Task WhenRequestIsSent()
        {
            State.Response = await HttpClient.SendAsync(State.Request);
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
                PropertyNameCaseInsensitive = true
            };
            var responseContent = await State.Response.Content.ReadAsStringAsync();

            responseContent.Should().NotBeNullOrEmpty();

            var obj = JsonSerializer.Deserialize<T>(responseContent, options);
            return obj;
        }

        [Given(@"Shipment Details in body")]
        public void GivenShipmentDetailsInBody()
        {
            var shipment = RequestDataGenerator.GenerateShipment();

            SetJsonBody(shipment);
        }
    }
}
