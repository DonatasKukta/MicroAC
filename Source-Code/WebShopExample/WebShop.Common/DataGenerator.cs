using System;
using System.Collections.Generic;

using Bogus;

namespace WebShop.Common
{
    public class DataGenerator
    {
        Faker<Order> FakeOrder;

        Faker<Order.Item> FakeOrderItem;

        Faker<Order.PaymentDetails> FakeOrderPaymentDetails;

        Faker<Shipment> FakeShipment;

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
        }

        public Order GenerateOrder()
        {
            return FakeOrder.Generate();
        }

        public IEnumerable<Order> GenerateOrders()
        {
            return FakeOrder.Generate(100);
        }

        public Order.PaymentDetails GenerateOrderPaymentDetails()
        {
            return FakeOrderPaymentDetails.Generate();
        }

        public Shipment GenerateShipment()
        {
            return FakeShipment.Generate();
        }

        public List<Shipment> GenerateShipments()
        {
            return FakeShipment.Generate(100);
        }
    }
}
