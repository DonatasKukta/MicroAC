using System;
using System.Collections.Generic;

namespace WebShop.Common
{
    public class Order
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public List<Item> Products { get; set; }
        public PaymentDetails Payment { get; set; }
        public Shipment Shipment { get; set; }
        
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
            public string CardNumber { get; set; }
            public string SecurityCode { get; set; }
            public string ExpiryDate { get; set; }
            public decimal TotalSum { get; set; }
        }
    }
}
