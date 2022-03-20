using System;
using System.Collections.Generic;

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
        public IEnumerable<Shipment> GetShipments()
        {
            return Data.GenerateShipments();
        }

        [HttpGet("/{shipmentId}")]
        public Shipment GetShipment([FromRoute] Guid shipmentId)
        {
            return Data.GenerateShipment();
        }

        [HttpPost("/{orderId}")]
        public ActionResult CreateShipment(
            [FromRoute] Guid orderId, 
            [FromBody] Shipment shipment)
        {
            return Created(Guid.NewGuid().ToString(), Data.GenerateShipment());
        }

        [HttpPut("/{shipmentId}")]
        public ActionResult UpdateShipment(
            [FromRoute] Guid shipmentId,
            [FromBody] Shipment shipmentDetails)
        {
            return Ok(shipmentDetails);
        }

        [HttpDelete("/{shipmentId}")]
        public ActionResult DeleteShipment([FromRoute] Guid shipmentId)
        {
            return Ok();
        }
    }
}
