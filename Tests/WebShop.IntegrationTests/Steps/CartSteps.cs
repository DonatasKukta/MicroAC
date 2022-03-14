using System;
using System.Threading.Tasks;

using FluentAssertions;

using TechTalk.SpecFlow;

using WebShop.IntegrationTests.Models;

namespace WebShop.IntegrationTests.Steps
{
    [Binding]
    [Scope(Feature = "Carts")]
    public sealed class CartSteps : SharedSteps
    {
        Cart? Cart;

        public CartSteps()
            : base(new Uri("http://localhost:19083/MicroAC.ServiceFabric/WebShop.Cart/carts"))
        {

        }

        [Given(@"cart id in url")]
        public void GivenCartIdInUrl()
        {
            AppendToRequestUrl(Guid.NewGuid().ToString());
        }

        [Given(@"new cart item")]
        public void GivenNewCartItem()
        {
            AppendToRequestUrl($"products");
            SetJsonBody(new Item { ProductId = 1, AddedAt = DateTime.Now, Quantity = 4 });
        }

        [Given(@"with product id")]
        public void GivenWithProductId()
        {
            AppendToRequestUrl($"products/{Guid.NewGuid()}");
        }

        [Then(@"the response contains a cart")]
        public async Task ThenTheResponseContainsACart()
        {
            Cart = await GetResponseJsonData<Cart>();

            Cart.Should().NotBeNull();
            Cart.Id.Should().NotBeEmpty();
        }

        [Then(@"a list of cart items")]
        public void ThenAListOfCartItems()
        {
            Cart.Items.Should().NotBeEmpty();
        }
    }
}