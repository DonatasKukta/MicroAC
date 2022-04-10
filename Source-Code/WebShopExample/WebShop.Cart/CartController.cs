using System;
using System.Collections.Generic;
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

        [HttpPost()]
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
        public async Task<WebShopCart> GetCart(Guid cartId)
        {
            await WebShopApi.SendServiceRequest(
                this.HttpContext,
                MicroACServices.Products,
                HttpMethod.Get);

            return Data.GenerateCart();
        }

        [HttpPost("/{cartId}/products")]
        public ActionResult AddCartItem([FromBody] CartItem newCartItem)
        {
            return Ok();
        }

        [HttpDelete("/{cartId}")]
        public ActionResult DeleteCart()
        {
            return Ok();
        }

        [HttpDelete("/{cartId}/products/{productId}")]
        public ActionResult DeleteCartItem()
        {
            return Ok();
        }
    }

}
