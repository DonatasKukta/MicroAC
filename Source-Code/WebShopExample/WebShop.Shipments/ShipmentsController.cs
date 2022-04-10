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
        [MicroAuth]
        public IEnumerable<Shipment> GetShipments()
        {
            return Data.GenerateShipments();
        }

        [HttpGet("/{shipmentId}")]
        [MicroAuth]
        public Shipment GetShipment([FromRoute] Guid shipmentId)
        {
            return Data.GenerateShipment();
        }

        [HttpPost("/{orderId}")]
        [MicroAuth]
        public async Task<ActionResult> CreateShipment(
            [FromRoute] Guid orderId, 
            [FromBody] Shipment shipment)
        {
            await WebShopApi.SendServiceRequest(
                this.HttpContext,
                MicroACServices.Products,
                HttpMethod.Get);

            return Created(Guid.NewGuid().ToString(), shipment);
        }

        [HttpPut("/{shipmentId}")]
        [MicroAuth]
        public async Task<ActionResult> UpdateShipment(
            [FromRoute] Guid shipmentId,
            [FromBody] Shipment shipmentDetails)
        {
            await WebShopApi.SendServiceRequest(
                this.HttpContext,
                MicroACServices.Products,
                HttpMethod.Get,
                $"/{shipmentId}");

            return Ok(shipmentDetails);
        }

        [HttpDelete("/{shipmentId}")]
        [MicroAuth]
        public async Task<ActionResult> DeleteShipment([FromRoute] Guid shipmentId)
        {
            await WebShopApi.SendServiceRequest(
                this.HttpContext,
                MicroACServices.Products,
                HttpMethod.Get);

            return Ok();
        }
    }
}
