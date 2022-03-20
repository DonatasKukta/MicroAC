using System;

namespace WebShop.Common
{
    public class Shipment
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid OrderId { get; set; }
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
