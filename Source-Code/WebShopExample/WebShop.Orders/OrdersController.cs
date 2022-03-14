using System;
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
            return new Order();
        }
    }
}
