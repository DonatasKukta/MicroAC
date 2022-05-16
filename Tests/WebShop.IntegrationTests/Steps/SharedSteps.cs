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
        protected readonly TestState State;

        protected readonly DataGenerator RequestDataGenerator;

        readonly HttpClient HttpClient;

        protected readonly IEndpointResolver EndpointResolver;

        protected IConfiguration Configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.core.json")
            .AddJsonFile("appsettings.json")
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
            State.Request.Headers.Add(HttpHeaders.Authorization, Configuration.GetValue<string>("TestAccessToken"));
            //EndpointResolver = new FabricEndpointResolver(Configuration);
            EndpointResolver = new FabricReverseProxyEndpointResolver(Configuration);
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
