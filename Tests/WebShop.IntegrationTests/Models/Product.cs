namespace WebShop.IntegrationTests.Models
{
    public class Product
    {
        public Product() { }

        public int Id { get; set; } = 1;
        public string Name { get; set; } = "StubName";
        public string Description { get; set; } = "StubDescription";
        public decimal Price { get; set; } = 1000m;
        public int Quantity { get; set; } = 100;
    }
}
