using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

using MicroAC.Core.Client;

using Microsoft.AspNetCore.Mvc;

using WebShop.Common;

namespace WebShop.Orders
{
    [Route("/")]
    public class OrdersController : Controller
    {
        readonly DataGenerator Data;

        readonly IWebShopApiClient WebShopApi;

        public OrdersController(IWebShopApiClient webShopApi)
        {
            WebShopApi = webShopApi;
            Data = new DataGenerator();
        }

        [HttpGet]
        [MicroAuth]
        public IEnumerable<Order> GetOrders()
        {
            return Data.GenerateOrders();
        }

        [HttpGet("/{id}")]
        [MicroAuth]
        public Order GetOrder([FromRoute] Guid id)
        {
            var order = Data.GenerateOrder();
            order.Id = id;
            return order;
        }

        [HttpPost("/{cartId}")]
        [MicroAuth]
        public async Task<ActionResult> CreateOrder([FromRoute] Guid cartId)
        {
            await WebShopApi.SendServiceRequest(
                this.HttpContext,
                MicroACServices.Cart,
                HttpMethod.Get,
                $"/{cartId}");

            await WebShopApi.SendServiceRequest(
                this.HttpContext,
                MicroACServices.Cart,
                HttpMethod.Delete,
                $"/{cartId}");

            await WebShopApi.SendServiceRequest(
                this.HttpContext,
                MicroACServices.Products,
                HttpMethod.Get,
                "/");

            var order = Data.GenerateOrder();
            
            order.Shipment = null;
            order.Payment = null;

            return Created(Guid.NewGuid().ToString(), order);
        }

        [HttpDelete("/{orderId}")]
        [MicroAuth]
        public ActionResult DeleteOrder([FromRoute] Guid orderId)
        {
            return Ok();
        }

        [HttpPut("/{orderId}/shipment")]
        [MicroAuth]
        public async Task<ActionResult> SubmitShipmentDetails(
            [FromRoute] Guid orderId, 
            [FromBody] Shipment shipmentDetails)
        {
            await WebShopApi.SendServiceRequest(
                this.HttpContext,
                MicroACServices.Shipments,
                HttpMethod.Put,
                $"/{orderId}",
                Data.GenerateShipment());
            
            return Created(orderId.ToString(), shipmentDetails);
        }

        [HttpPut("/{orderId}/payment")]
        [MicroAuth]
        public async Task<ActionResult> SubmitPaymentDetails(
            [FromRoute] Guid orderId,
            [FromBody] Order.PaymentDetails paymentDetails)
        {
            await WebShopApi.SendServiceRequest(
                this.HttpContext,
                MicroACServices.Shipments,
                HttpMethod.Put,
                $"/{orderId}",
                Data.GenerateShipment());

            return Created(orderId.ToString(), paymentDetails);
        }

        [HttpPut("/{orderId}")]
        [MicroAuth]
        public async Task<Order> SubmitOrder([FromRoute] Guid orderId)
        {
            await WebShopApi.SendServiceRequest(
                this.HttpContext,
                MicroACServices.Products,
                HttpMethod.Get,
                $"/");

            await WebShopApi.SendServiceRequest(
                this.HttpContext,
                MicroACServices.Shipments,
                HttpMethod.Put,
                $"/{Guid.NewGuid()}",
                Data.GenerateShipment());

            return GetOrder(orderId);
        }
    }
}
