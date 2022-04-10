using System;
using System.Collections.Generic;

using Bogus;

namespace WebShop.Common
{
    public class DataGenerator
    {
        readonly Faker<Order> FakeOrder;
         
        readonly Faker<Order.Item> FakeOrderItem;
         
        readonly Faker<Order.PaymentDetails> FakeOrderPaymentDetails;
         
        readonly Faker<Shipment> FakeShipment;
         
        readonly Faker<WebShopCart> FakeCart;
         
        readonly Faker<CartItem> FakeCartItem;
         
        readonly Faker<Product> FakeProduct;

        public DataGenerator()
        {
            FakeOrderItem = new Faker<Order.Item>()
                .RuleFor(i => i.ProductId, f => f.Random.Int(1, 20))
                .RuleFor(i => i.AddedAt, f => f.Date.Recent(0))
                .RuleFor(i => i.Quantity, f => f.Random.Int(1, 5))
                .RuleFor(i => i.Price, f => f.Random.Decimal(250, 1700))
                .RuleFor(i => i.Discount, f => f.Random.Decimal(0.05m, 0.3m).OrNull(f));

            FakeOrderPaymentDetails = new Faker<Order.PaymentDetails>()
                .RuleFor(i => i.TotalSum, f => f.Finance.Amount())
                .RuleFor(i => i.SecurityCode, "***")
                .RuleFor(i => i.ExpiryDate, "**/**")
                .RuleFor(i => i.CardNumber, f =>
                 new string('*', 12) + f.Finance.CreditCardNumber().Substring(11));

            FakeShipment = new Faker<Shipment>()
                .RuleForType(typeof(Guid), f => f.Random.Guid())
                .RuleFor(i => i.Country, f => f.Address.Country())
                .RuleFor(i => i.AddressLine1, f => f.Address.StreetAddress())
                .RuleFor(i => i.AddressLine2, f => f.Address.SecondaryAddress())
                .RuleFor(i => i.City, f => f.Address.City())
                .RuleFor(i => i.PostCode, f => f.Address.ZipCode())
                .RuleFor(i => i.Cost, f => f.Random.Decimal(4m, 30m))
                .RuleFor(i => i.Provider, f => f.Company.CompanyName());

            FakeOrder = new Faker<Order>()
                .RuleForType(typeof(Guid), f => f.Random.Guid())
                .RuleFor(i => i.Products, f => FakeOrderItem.GenerateBetween(1, 10))
                .RuleFor(i => i.Payment, f => FakeOrderPaymentDetails.Generate())
                .RuleFor(i => i.Shipment, f => FakeShipment.Generate());

            FakeCartItem = new Faker<CartItem>()
                .RuleForType(typeof(Guid), f => f.Random.Guid())
                .RuleFor(i => i.Quantity, f => f.Commerce.Random.Int(1, 10))
                .RuleFor(i => i.AddedAt, f => f.Date.Recent(1));

            FakeCart = new Faker<WebShopCart>()
                .RuleForType(typeof(Guid), f => f.Random.Guid())
                .RuleFor(i => i.Items, f => FakeCartItem.GenerateBetween(3, 15));

            FakeProduct = new Faker<Product>()
                .RuleForType(typeof(Guid), f => f.Random.Guid())
                .RuleFor(i => i.Name, f => f.Commerce.Product())
                .RuleFor(i => i.Description, f => f.Commerce.ProductDescription())
                .RuleFor(i => i.Price, f => f.Random.Decimal(50, 2000))
                .RuleFor(i => i.Quantity, f => f.Random.Int(3, 1000));
        }

        public Product GenerateProduct() => FakeProduct.Generate();
        public IEnumerable<Product> GenerateProducts() => FakeProduct.GenerateBetween(10, 100);

        public Order GenerateOrder() => FakeOrder.Generate();
        public IEnumerable<Order> GenerateOrders() => FakeOrder.GenerateBetween(5, 100);
        public Order.PaymentDetails GenerateOrderPaymentDetails() => FakeOrderPaymentDetails.Generate();

        public Shipment GenerateShipment() => FakeShipment.Generate();
        public List<Shipment> GenerateShipments() => FakeShipment.GenerateBetween(10, 100);

        public WebShopCart GenerateCart() => FakeCart.Generate();
        public CartItem GenerateCartItem() => FakeCartItem.Generate();
    }
}
