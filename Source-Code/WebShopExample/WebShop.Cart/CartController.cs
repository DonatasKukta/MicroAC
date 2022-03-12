using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebShop.Cart
{
    [Route("/")]
    public class CartController : Controller
    {
        public CartController()
        {

        }

        [HttpPost()]
        public WebShopCart CreateCart()
        {
            return new WebShopCart()
            {
                Id = Guid.NewGuid(),
                UserId = Guid.Empty,
                Items = new List<CartItem>()
            };
        }

        [HttpGet("/{cartId}")]
        public WebShopCart GetCart(Guid cartId)
        {
            return new WebShopCart()
            {
                Id = cartId,
                UserId = Guid.Empty,
                Items = Items
            };
        }

        [HttpPost("/{cartId}/products")]
        public ActionResult AddCartItem([FromBody] CartItem newCartItem) {
            return Ok();
        }

        [HttpDelete("cart/{cartId}")]
        public ActionResult DeleteCart()
        {
            return Ok();
        }

        [HttpDelete("cart/{cartId}/products/{productId}")]
        public ActionResult DeleteCartItem()
        {
            return Ok();
        }

        List<CartItem> Items = new List<CartItem>
            {
                new CartItem { ProductId = 1, Quantity = 4, AddedAt = DateTime.Now},
                new CartItem { ProductId = 3, Quantity = 2, AddedAt = DateTime.Now.AddMinutes(-5)},
                new CartItem { ProductId = 1, Quantity = 4, AddedAt = DateTime.Now.AddMinutes(-1)},
                new CartItem { ProductId = 1, Quantity = 6, AddedAt = DateTime.Now.AddMinutes(-20)},
                new CartItem { ProductId = 1, Quantity = 3, AddedAt = DateTime.Now.AddMinutes(-3)}
            };
    }

}
