using System;
using System.Collections.Generic;

namespace WebShop.Cart
{
    public class WebShopCart
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public List<CartItem> Items { get; set; }
    }

    public class CartItem
    {
        public int ProductId { get; set; }
        public DateTime AddedAt { get; set; }
        public int Quantity { get; set; }
    }
}
