using System;
using System.Collections.Generic;
using System.Net.Http;

using Microsoft.AspNetCore.Mvc;

using WebShop.Common;

namespace WebShop.Orders
{
    [Route("/")]
    public class OrdersController : Controller
    {
        DataGenerator Data;

        IWebShopApiClient WebShopApi;

        public OrdersController(IWebShopApiClient webShopApi)
        {
            Data = new DataGenerator();
            WebShopApi = webShopApi;
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
            WebShopApi.SendServiceRequest(
                WebShopServices.Cart,
                HttpMethod.Get,
                $"/carts/{cartId}");

            WebShopApi.SendServiceRequest(
                WebShopServices.Cart,
                HttpMethod.Delete,
                $"/carts/{cartId}");

            WebShopApi.SendServiceRequest(
                WebShopServices.Products,
                HttpMethod.Get,
                "/");

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
            WebShopApi.SendServiceRequest(
                WebShopServices.Shipments,
                HttpMethod.Post,
                "/");
            
            return Created(orderId.ToString(), shipmentDetails);
        }

        [HttpPut("/{orderId}/payment")]
        public ActionResult SubmitPaymentDetails(
            [FromRoute] Guid orderId,
            [FromBody] Order.PaymentDetails paymentDetails)
        {
            return Created(orderId.ToString(), paymentDetails);
        }

        [HttpPut("/{orderId}")]
        public Order SubmitOrder([FromRoute] Guid orderId)
        {
            WebShopApi.SendServiceRequest(
                WebShopServices.Products,
                HttpMethod.Get,
                $"/");

            WebShopApi.SendServiceRequest(
                WebShopServices.Shipments,
                HttpMethod.Put,
                $"/{Guid.NewGuid()}",
                "",
                Data.GenerateShipment());

            return GetOrder(orderId);
        }
    }
}
