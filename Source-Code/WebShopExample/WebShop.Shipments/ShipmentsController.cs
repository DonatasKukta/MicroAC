using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

using MicroAC.Core.Client;

using Microsoft.AspNetCore.Mvc;

using WebShop.Common;

namespace WebShop.Shipments
{
    [Route("/")]
    public class ShipmentsController : Controller
    {
        IWebShopApiClient WebShopApi;
        DataGenerator Data;

        public ShipmentsController(IWebShopApiClient webShopApi)
        {
            WebShopApi = webShopApi;
            Data = new DataGenerator();
        }

        [HttpGet]
        [MicroAuth(ServiceName = "Shipments", Action = "View", Value = "All")]
        public IEnumerable<Shipment> GetShipments()
        {
            return Data.GenerateShipments();
        }

        [HttpGet("/{shipmentId}")]
        [MicroAuth(ServiceName = "Shipments", Action = "View", Value = "One")]
        public Shipment GetShipment([FromRoute] Guid shipmentId)
        {
            return Data.GenerateShipment();
        }

        [HttpPost("/{orderId}")]
        [MicroAuth(ServiceName = "Shipments", Action = "Create")]
        public async Task<ActionResult> CreateShipment(
            [FromRoute] Guid orderId, 
            [FromBody] Shipment shipment)
        {
            await WebShopApi.SendServiceRequest(
                this.HttpContext,
                MicroACServices.Products,
                HttpMethod.Post,
                "",
                Data.GenerateProduct());

            return Created(Guid.NewGuid().ToString(), shipment);
        }

        [HttpPut("/{shipmentId}")]
        [MicroAuth(ServiceName = "Shipments", Action = "Update")]
        public async Task<ActionResult> UpdateShipment(
            [FromRoute] Guid shipmentId,
            [FromBody] Shipment shipmentDetails)
        {
            await WebShopApi.SendServiceRequest(
                this.HttpContext,
                MicroACServices.Products,
                HttpMethod.Put,
                $"/{shipmentId}",
                Data.GenerateProduct());

            return Ok(shipmentDetails);
        }

        [HttpDelete("/{shipmentId}")]
        [MicroAuth(ServiceName = "Shipments", Action = "Delete")]
        public async Task<ActionResult> DeleteShipment([FromRoute] Guid shipmentId)
        {
            await WebShopApi.SendServiceRequest(
                this.HttpContext,
                MicroACServices.Products,
                HttpMethod.Delete,
                $"/{shipmentId}");

            return Ok();
        }
    }
}
