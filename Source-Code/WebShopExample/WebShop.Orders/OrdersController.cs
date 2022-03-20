using System;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;

using WebShop.Common;

namespace WebShop.Orders
{
    [Route("/")]
    public class OrdersController : Controller
    {
        DataGenerator Data;

        public OrdersController()
        {
            Data = new DataGenerator();
        }

        [HttpGet]
        public IEnumerable<Order> GetOrders()
        {
            return Data.GenerateOrders();
        }

        [HttpGet("/{id}")]
        public Order GetOrder([FromRoute] Guid id)
        {
            var order = Data.GenerateOrder();
            order.Id = id;
            return order;
        }

        [HttpPost("/{cartId}")]
        public ActionResult CreateOrder([FromRoute] Guid cartId)
        {
            // Call Carts Api (Get)
            // Call Carts Api (Delete)
            // Call Products Api

            var order = Data.GenerateOrder();
            
            order.Shipment = null;
            order.Payment = null;

            return Created(Guid.NewGuid().ToString(), order);
        }

        [HttpDelete("/{orderId}")]
        public ActionResult DeleteOrder([FromRoute] Guid orderId)
        {
            return Ok();
        }

        [HttpPut("/{orderId}/shipment")]
        public ActionResult SubmitShipmentDetails(
            [FromRoute] Guid orderId, 
            [FromBody] Shipment shipmentDetails)
        {
            // Call Shipment Api
            return Created(Guid.NewGuid().ToString(), shipmentDetails);
        }

        [HttpPut("/{orderId}/payment")]
        public ActionResult SubmitPaymentDetails(
            [FromRoute] Guid orderId,
            [FromBody] Order.PaymentDetails paymentDetails)
        {
            return Created(Guid.NewGuid().ToString(), paymentDetails);
        }

        [HttpPut("/{orderId}")]
        public Order SubmitOrder([FromRoute] Guid orderId)
        {
            // Call Shipment Api
            // Call Payment Api
            // Call Products Api
            return GetOrder(orderId);
        }
    }
}
