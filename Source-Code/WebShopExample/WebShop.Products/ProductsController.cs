using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

using MicroAC.Core.Client;

using Microsoft.AspNetCore.Mvc;

using WebShop.Common;

namespace WebShop.Products
{
    [Route("/")]
    public class ProductsController : Controller
    {
        readonly DataGenerator Data;

        readonly IWebShopApiClient WebShopApi;

        public ProductsController(IWebShopApiClient webShopApi)
        {
            WebShopApi = webShopApi;
            Data = new DataGenerator();
        }

        [HttpGet]
        [MicroAuth]
        public IEnumerable<Product> GetProducts()
        {
            return Data.GenerateProducts();
        }

        [HttpGet("/{id}")]
        [MicroAuth]
        public Product GetProduct([FromRoute] Guid id)
        {
            return Data.GenerateProduct();
        }

        [HttpPost]
        [MicroAuth]
        public ActionResult CreateProduct([FromBody] Product product)
        {
            return product == null
                ? BadRequest("Product is missing from body.")
                : Created($"/products/{product.Id}", product);
        }

        [HttpPut("/{id}")]
        [MicroAuth]
        public async Task<ActionResult> UpdateProduct([FromRoute] Guid id, [FromBody] Product product)
        {
            await WebShopApi.SendServiceRequest(
                this.HttpContext,
                MicroACServices.Cart,
                HttpMethod.Post,
                $"/{id}/products",
                Data.GenerateCartItem());

            return Ok();
        }

        [HttpDelete("/{id}")]
        [MicroAuth]
        public async Task<ActionResult> DeleteProduct(Guid id)
        {
            await WebShopApi.SendServiceRequest(
                this.HttpContext,
                MicroACServices.Cart,
                HttpMethod.Post,
                $"/{id}/products",
                Data.GenerateCartItem());

            return Ok();
        }
    }
}
