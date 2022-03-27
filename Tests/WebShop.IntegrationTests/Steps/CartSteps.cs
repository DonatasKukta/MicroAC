using System;
using System.Threading.Tasks;

using FluentAssertions;

using TechTalk.SpecFlow;

using WebShop.Common;

namespace WebShop.IntegrationTests.Steps
{
    [Binding]
    [Scope(Feature = "Carts")]
    public sealed class CartSteps : SharedSteps
    {
        WebShopCart? Cart;

        public CartSteps() : base("Carts")
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
            SetJsonBody(new CartItem { ProductId = 1, AddedAt = DateTime.Now, Quantity = 4 });
        }

        [Given(@"with product id")]
        public void GivenWithProductId()
        {
            AppendToRequestUrl($"products/{Guid.NewGuid()}");
        }

        [Then(@"the response contains a cart")]
        public async Task ThenTheResponseContainsACart()
        {
            Cart = await GetResponseJsonData<WebShopCart>();

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