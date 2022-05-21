using System;
using System.Net.Http;
using System.Threading.Tasks;

using MicroAC.Core.Client;

using Microsoft.AspNetCore.Mvc;

using WebShop.Common;

namespace WebShop.Cart
{
    [Route("/")]
    public class CartController : Controller
    {
        readonly DataGenerator Data;

        readonly IWebShopApiClient WebShopApi;

        public CartController(IWebShopApiClient webShopApi)
        {
            WebShopApi = webShopApi;
            Data = new DataGenerator();
        }

        [HttpPost]
        [MicroAuth(ServiceName = "Carts", Action = "Create")]
        public async Task<WebShopCart> CreateCart()
        {
            await WebShopApi.SendServiceRequest(
                this.HttpContext,
                MicroACServices.Products,
                HttpMethod.Get,
                $"/{Guid.NewGuid()}");

            return Data.GenerateCart();
        }

        [HttpGet("/{cartId}")]
        [MicroAuth(ServiceName = "Carts", Action = "Get", Value = "Self")]
        public async Task<WebShopCart> GetCart(Guid cartId)
        {
            await WebShopApi.SendServiceRequest(
                this.HttpContext,
                MicroACServices.Products,
                HttpMethod.Get);

            return Data.GenerateCart();
        }

        [HttpPost("/{cartId}/products")]
        [MicroAuth(ServiceName = "Carts", Action = "Update", Value = "CartItem")]
        public ActionResult AddCartItem([FromBody] CartItem newCartItem)
        {
            return Ok();
        }

        [HttpDelete("/{cartId}")]
        [MicroAuth(ServiceName = "Carts", Action = "Delete", Value = "Cart")]
        public ActionResult DeleteCart()
        {
            return Ok();
        }

        [HttpDelete("/{cartId}/products/{productId}")]
        [MicroAuth(ServiceName = "Carts", Action = "Delete", Value = "CartItem")]
        public ActionResult DeleteCartItem()
        {
            return Ok();
        }
    }
}
