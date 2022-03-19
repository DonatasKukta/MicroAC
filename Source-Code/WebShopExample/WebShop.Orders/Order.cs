using System;
using System.Collections.Generic;

namespace WebShop.Orders
{
    public class Order
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public List<Item> Products { get; set; }
        public PaymentDetails PaymentDetails { get; set; }
        public ShipmentDetails ShipmentDetails { get; set; }
    }

    public class Item
    {
        public int ProductId { get; set; }
        public DateTime AddedAt { get; set; }
        public int Quantity { get; set; }
        public decimal? Price { get; set; }
        public decimal? Discount { get; set; }
    }

    public class PaymentDetails
    {
        //TODO: Limit exposure
        public string CardNumber { get; set; }
        public string SecurityCode { get; set; }
        public string ExpiryDate { get; set; }
        public decimal TotalSum { get; set; }
    }

    public class ShipmentDetails
    {
        public string Country { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string PostCode { get; set; }
        public decimal Cost { get; set; }
        public string Provider { get; set; }
        public string Type { get; set; }
    }
}
