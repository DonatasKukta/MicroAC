﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using FluentAssertions;

using TechTalk.SpecFlow;

using WebShop.Common;

namespace WebShop.IntegrationTests.Steps
{
    [Binding]
    [Scope(Feature = "Shipments")]
    public sealed class ShipmentsSteps : SharedSteps
    {
        readonly DataGenerator RequestDataGenerator;

        public ShipmentsSteps()
            : base(new Uri("http://localhost:19083/MicroAC.ServiceFabric/WebShop.Shipments"))
        {
            RequestDataGenerator = new DataGenerator();
        }

        [Given(@"(.*) in path")]
        public void GivenGuidInPath(string pathParameter)
        {
            if (pathParameter == "Guid")
                this.AppendToRequestUrl(Guid.NewGuid().ToString());
            else
                this.AppendToRequestUrl(pathParameter);
        }

        [Given(@"Shipment in Body")]
        public void GivenShipmentInBody()
        {
            var shipment = RequestDataGenerator.GenerateShipment();

            SetJsonBody(shipment);
        }


        [Then(@"response contains a list of Shipments")]
        public async Task ThenResponseContainsAListOfShipments()
        {
            var shipments = await GetResponseJsonData<List<Shipment>>();

            shipments.Should().NotBeNull();
            shipments.Should().NotBeEmpty();
        }

        [Then(@"response contains a Shipment")]
        public async Task ThenResponseContainsAShipment()
        {
            var shipment = await GetResponseJsonData<Shipment>();

            shipment.Should().NotBeNull();
        }
    }
}
