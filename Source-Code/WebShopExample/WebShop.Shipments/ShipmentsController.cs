using System;
using System.Collections.Generic;

using MicroAC.Core.Client;

using Microsoft.AspNetCore.Mvc;

using WebShop.Common;

namespace WebShop.Shipments
{
    [Route("/")]
    public class ShipmentsController : Controller
    {
        DataGenerator Data;

        public ShipmentsController()
        {
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
        public ActionResult CreateShipment(
            [FromRoute] Guid orderId, 
            [FromBody] Shipment shipment)
        {
            return Created(Guid.NewGuid().ToString(), shipment);
        }

        [HttpPut("/{shipmentId}")]
        [MicroAuth]
        public ActionResult UpdateShipment(
            [FromRoute] Guid shipmentId,
            [FromBody] Shipment shipmentDetails)
        {
            return Ok(shipmentDetails);
        }

        [HttpDelete("/{shipmentId}")]
        [MicroAuth]
        public ActionResult DeleteShipment([FromRoute] Guid shipmentId)
        {
            return Ok();
        }
    }
}
