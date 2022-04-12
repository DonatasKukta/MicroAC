using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using FluentAssertions;

using MicroAC.Core.Client;
using MicroAC.Core.Constants;

using Microsoft.Extensions.Configuration;

using TechTalk.SpecFlow;

using WebShop.Common;

namespace WebShop.IntegrationTests.Steps
{
    public class SharedSteps
    {
        //TODO: Move to config
        static readonly string TestAuthToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiI0YzlkZjM3MS1lZjVlLTRhOTgtYTllNC00ODUyNDA3ZDI0MGEiLCJpc3MiOiJNaWNyb0FDOkF1dGhlbnRpY2F0aW9uU2VydmljZSIsImF1ZCI6Ik1pY3JvQUM6QXV0aG9yaXphdGlvblNlcnZpY2UiLCJzdWIiOiJNaWNyb0FDOlVzZXIiLCJ1aWQiOiJmZTk4MzEwYi1lYmMyLTQyOTktOTg1NC1mYmVhNDZmNjI1OTEiLCJ1cm9sZXMiOlsiUm9sZTEwX1NlZWRUZXN0RGF0YSJdLCJuYmYiOjE2NDc3OTk4MDQsImV4cCI6MTY2NTA3OTgwNCwiaWF0IjoxNjQ3Nzk5ODA0fQ.zeEV6S0dbzNZMwTBxadf34WJzXPjTmFBzx7s_EAOCVA";

        protected readonly TestState State;

        protected readonly DataGenerator RequestDataGenerator;

        readonly HttpClient HttpClient;

        readonly IEndpointResolver EndpointResolver;

        IConfiguration Configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.core.json")
            .AddEnvironmentVariables()
            .Build();

        public SharedSteps(MicroACServices service)
        {
            HttpClient = new HttpClient();
            RequestDataGenerator = new DataGenerator();
            State = new TestState
            {
                Request = new HttpRequestMessage(),
                Response = new HttpResponseMessage()
            };
            State.Request.Headers.Add(HttpHeaders.Authorization, TestAuthToken);
            EndpointResolver = new FabricEndpointResolver();
            EndpointResolver.InitialiseEndpoints();
            State.Url = RetrieveUrl(service);
        }

        [Given(@"(.*) request")]
        public void GivenGetProductsRequest(string method)
        {
            State.Request.Method = new HttpMethod(method);
        }

        [When(@"request is sent")]
        public async Task WhenRequestIsSent()
        {
            State.Request.RequestUri = new Uri(this.State.Url);
            State.Response = await HttpClient.SendAsync(State.Request);
        }

        [Then(@"response status code is (.*)")]
        public void ThenResponseStatusCodeIs(int status)
        {
            State.Response.StatusCode.Should().Be((HttpStatusCode)status);
        }

        protected void AppendToRequestUrl(string uri)
        {
            this.State.Url = $"{this.State.Url}/{uri}";
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

        string RetrieveUrl(MicroACServices service)
        {
            var IsCentralAuthorizationEnabled = Configuration.GetValue<bool>(ConfigKeys.CentralAuthorizationEnabled);
            var url = IsCentralAuthorizationEnabled
                ? EndpointResolver.GetServiceEndpoint(MicroACServices.RequestManager) 
                    + "/"
                    + Enum.GetName(typeof(MicroACServices), service)
                : EndpointResolver.GetServiceEndpoint(service);
            return url ?? string.Empty;
        }
    }
}
