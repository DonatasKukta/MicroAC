using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using FluentAssertions;
using TechTalk.SpecFlow;
using System.Collections.Generic;
using WebShop.IntegrationTests.Models;
using System.Text.Json;
using System.Text;

namespace WebShop.IntegrationTests.Steps
{
    [Binding]
    public sealed class ProductsSteps
    {
        readonly Uri Url = new Uri("http://localhost:19083/MicroAC.ServiceFabric/WebShop.Products/");

        readonly TestState State;

        readonly HttpClient HtppClient = new HttpClient();

        public ProductsSteps()
        {
            State = new TestState
            {
                Request = new HttpRequestMessage()
                {
                    RequestUri = Url
                },
                Response = new HttpResponseMessage()
            };
        }

        [Given(@"(.*) request")]
        public void GivenGetProductsRequest(string method)
        {
            State.Request.Method = new HttpMethod(method);
        }

        [Given(@"product Id")]
        public void GivenRequestWithProductId()
        {
            State.Request.RequestUri = new Uri(Url.ToString() + "1");
        }

        [Given(@"product")]
        public void GivenProduct()
        {
            var productStr = JsonSerializer.Serialize(new Product());
            State.Request.Content = new StringContent(
                productStr, 
                Encoding.UTF8, 
                "application/json"
                );
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

        [Then(@"the response contains a list of products")]
        public async Task ThenTheResponseContainsAListOfProducts()
        {
            var products = await State.GetResponseJsonData<List<Product>>();
            products.Should().NotBeNull();
            products.Count.Should().BeGreaterThan(0);
        }

        [Then(@"the response contains a product")]
        public async Task ThenTheResponseContainsAProduct()
        {
            var product = await State.GetResponseJsonData<Product>();
            product.Should().NotBeNull();
            product.Name.Length.Should().BeGreaterThan(1);
        }
    }
}
