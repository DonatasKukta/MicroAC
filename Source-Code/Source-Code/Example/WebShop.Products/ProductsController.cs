using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Mvc;

namespace WebShop.Products
{
    [Route("/")]
    public class ProductsController : Controller
    {
        readonly List<Product> Products = new()
        {
            new Product { Id = 1, Price = 900, Quantity = 10, Name = "HP Pavilion 24 All-in-One (2020)" },
            new Product { Id = 2, Price = 2100, Quantity = 6, Name = "HP Envy 34 All-in-One" },
            new Product { Id = 3, Price = 1750, Quantity = 40, Name = "Apple iMac 27-Inch" },
            new Product { Id = 4, Price = 500, Quantity = 26, Name = "HP Chromebase All-in-One 22" },
            new Product { Id = 5, Price = 400, Quantity = 4, Name = "Dell Inspiron Desktop (3891)", },
            new Product { Id = 6, Price = 4600, Quantity = 16, Name = "Velocity Micro Raptor Z55", },
            new Product { Id = 7, Price = 2700, Quantity = 23, Name = "Dell Precision 7920 Tower", },
            new Product { Id = 8, Price = 1000, Quantity = 1, Name = "Lenovo Legion Tower 5i", },
            new Product { Id = 9, Price = 1900, Quantity = 0, Name = "HP Omen 45L", },
            new Product { Id = 10, Price = 1500, Quantity = 9, Name = "Alienware Aurora R13", },
        };

        public ProductsController() { }

        [HttpGet]
        public IEnumerable<Product> GetProducts()
        {
            return Products;
        }

        [HttpGet("/{id}")]
        public Product GetProduct([FromRoute] int id)
        {
            return Products.FirstOrDefault(p => p.Id == id % Products.Count);
        }

        [HttpPost]
        public ActionResult CreateProduct([FromBody] Product product)
        {
            return Created($"/products/{product.Id}", product);
        }

        [HttpPut]
        public ActionResult UpdateProduct()
        {
            return Ok();
        }

        [HttpDelete("/{id}")]
        public ActionResult DeleteProduct([FromRoute] int id)
        {
            return Ok();
        }
    }
}
