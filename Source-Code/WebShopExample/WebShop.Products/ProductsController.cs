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
        [MicroAuth(ServiceName = "Products", Action = "View", Value = "All")]
        public IEnumerable<Product> GetProducts()
        {
            return Data.GenerateProducts();
        }

        [HttpGet("/{id}")]
        [MicroAuth(ServiceName = "Products", Action = "View", Value = "One")]
        public Product GetProduct([FromRoute] Guid id)
        {
            return Data.GenerateProduct();
        }

        [HttpPost]
        [MicroAuth(ServiceName = "Products", Action = "Create")]
        public ActionResult CreateProduct([FromBody] Product product)
        {
            return product == null
                ? BadRequest("Product is missing from body.")
                : Created($"/products/{product.Id}", product);
        }

        [HttpPut("/{id}")]
        [MicroAuth(ServiceName = "Products", Action = "Update")]
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
        [MicroAuth(ServiceName = "Products", Action = "Delete")]
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
