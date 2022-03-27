using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using WebShop.Common;

namespace WebShop.Cart
{
    [Route("/")]
    public class CartController : Controller
    {
        readonly IWebShopApiClient WebShopApi;

        public CartController(IWebShopApiClient webShopApi)
        {
            WebShopApi = webShopApi;
        }

        [HttpPost("/carts")]
        public async Task<WebShopCart> CreateCart()
        {
            await WebShopApi.SendServiceRequest(
                WebShopServices.Products,
                HttpMethod.Get,
                $"/");

            return new WebShopCart()
            {
                Id = Guid.NewGuid(),
                UserId = Guid.Empty,
                Items = new List<CartItem>()
            };
        }

        [HttpGet("/carts/{cartId}")]
        public WebShopCart GetCart(Guid cartId)
        {
            return new WebShopCart()
            {
                Id = cartId,
                UserId = Guid.Empty,
                Items = Items
            };
        }

        [HttpPost("/carts/{cartId}/products")]
        public ActionResult AddCartItem([FromBody] CartItem newCartItem) {
            return Ok();
        }

        [HttpDelete("/carts/{cartId}")]
        public ActionResult DeleteCart()
        {
            return Ok();
        }

        [HttpDelete("/carts/{cartId}/products/{productId}")]
        public ActionResult DeleteCartItem()
        {
            return Ok();
        }

        List<CartItem> Items = new()
            {
                new CartItem { ProductId = 1, Quantity = 4, AddedAt = DateTime.Now},
                new CartItem { ProductId = 3, Quantity = 2, AddedAt = DateTime.Now.AddMinutes(-5)},
                new CartItem { ProductId = 1, Quantity = 4, AddedAt = DateTime.Now.AddMinutes(-1)},
                new CartItem { ProductId = 1, Quantity = 6, AddedAt = DateTime.Now.AddMinutes(-20)},
                new CartItem { ProductId = 1, Quantity = 3, AddedAt = DateTime.Now.AddMinutes(-3)}
            };
    }

}
