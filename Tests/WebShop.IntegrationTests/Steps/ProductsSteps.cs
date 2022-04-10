using System;
using System.Threading.Tasks;

using FluentAssertions;
using TechTalk.SpecFlow;
using System.Collections.Generic;

using WebShop.Common;
using MicroAC.Core.Client;

namespace WebShop.IntegrationTests.Steps
{
    [Binding]
    [Scope(Feature = "Products")]
    public sealed class ProductsSteps : SharedSteps
    {
        public ProductsSteps() : base(MicroACServices.Products)
        {

        }

        [Given(@"product Id")]
        public void GivenRequestWithProductId()
        {
            AppendToRequestUrl(Guid.NewGuid().ToString());
        }

        [Given(@"product")]
        public void GivenProduct()
        {
            SetJsonBody(RequestDataGenerator.GenerateProduct());
        }

        [Then(@"the response contains a list of products")]
        public async Task ThenTheResponseContainsAListOfProducts()
        {
            var products = await GetResponseJsonData<List<Product>>();
            products.Should().NotBeNull();
            products.Should().NotBeEmpty();
        }

        [Then(@"the response contains a product")]
        public async Task ThenTheResponseContainsAProduct()
        {
            var product = await GetResponseJsonData<Product>();
            
            product.Should().NotBeNull();
            product.Name.Should().NotBeEmpty();
        }
    }
}
