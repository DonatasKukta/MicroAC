using System;
using System.Collections.Generic;

namespace WebShop.IntegrationTests.Models
{
    public class Cart
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public List<Item> Items { get; set; }
    }

    public class Item
    {
        public int ProductId { get; set; }
        public DateTime AddedAt { get; set; }
        public int Quantity { get; set; }
    }
}
