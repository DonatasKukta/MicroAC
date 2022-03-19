using System;

using Bogus;
using Microsoft.AspNetCore.Mvc;

namespace WebShop.Orders
{
    [Route("/")]
    public class OrdersController : Controller
    {
        public OrdersController()
        {

        }

        [HttpGet]
        public Order GetOrder()
        {
            return CreateFakeOrder();
        }

        Order CreateFakeOrder()
        {
            var fakeItem = new Faker<Item>()
                .RuleFor(i => i.ProductId, f => f.Random.Int(1, 20))
                .RuleFor(i => i.AddedAt, f => f.Date.Recent(0))
                .RuleFor(i => i.Quantity, f => f.Random.Int(1,5))
                .RuleFor(i => i.Price, f => f.Random.Decimal(250, 1700))
                .RuleFor(i => i.Discount, f => f.Random.Decimal(0.05m, 0.3m).OrNull(f));

            var fakePayemnt = new Faker<PaymentDetails>()
                .RuleFor(i => i.CardNumber, f => f.Finance.CreditCardNumber())
                .RuleFor(i => i.SecurityCode, f => f.Finance.CreditCardCvv())
                .RuleFor(i => i.ExpiryDate, f => f.Date.Past(3).ToString())
                .RuleFor(i => i.TotalSum, f => f.Finance.Amount());

            var fakeShipmentDetails = new Faker<ShipmentDetails>()
                .RuleFor(i => i.Country, f => f.Address.Country())
                .RuleFor(i => i.AddressLine1, f => f.Address.StreetAddress())
                .RuleFor(i => i.AddressLine2, f => f.Address.SecondaryAddress())
                .RuleFor(i => i.City, f => f.Address.City())
                .RuleFor(i => i.PostCode, f => f.Address.ZipCode())
                .RuleFor(i => i.Cost, f => f.Random.Decimal(4m, 30m))
                .RuleFor(i => i.Provider, f => f.Company.CompanyName());

            return new Faker<Order>()
                .RuleForType(typeof(Guid), f => f.Random.Guid())
                .RuleFor(i => i.Products, f => fakeItem.GenerateBetween(1, 10))
                .RuleFor(i => i.PaymentDetails, f => fakePayemnt.Generate())
                .RuleFor(i => i.ShipmentDetails, f => fakeShipmentDetails.Generate())
                .Generate();
        }
    }
}
