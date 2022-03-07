using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;

namespace WebShop.Products
{
    [Route("/Products")]
    public class ProductsController : Controller
    {
        public ProductsController() { }

        [HttpGet]
        public IEnumerable<Product> GetProducts()
        {
            var n = 10;
            var products = new List<Product>(n);
            for (var i = 0; i < n; i++)
            {
                products.Add(new Product
                {
                    Id = i,
                    Name = $"ProductName{i}",
                    Description = $"Lorem ipsum",
                    Price = i * i
                });
            }
            return products;
        }

        public class Product
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public decimal Price { get; set; }
        }
    }
}
