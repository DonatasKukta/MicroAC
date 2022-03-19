using System;
using System.Threading.Tasks;

using FluentAssertions;

using TechTalk.SpecFlow;

using WebShop.Common;

namespace WebShop.IntegrationTests.Steps
{
    [Binding]
    [Scope(Feature = "Orders")]
    public sealed class OrdersSteps : SharedSteps
    {
        readonly DataGenerator RequestDataGenerator;

        public OrdersSteps()
            : base(new Uri("http://localhost:19083/MicroAC.ServiceFabric/WebShop.Orders"))
        {
            RequestDataGenerator = new DataGenerator();
        }

        [Given(@"(.*) in path")]
        public void GivenGuidInPath(string pathParameter)
        {
            if(pathParameter == "Guid")
                this.AppendToRequestUrl(Guid.NewGuid().ToString());
            else
                this.AppendToRequestUrl(pathParameter);
        }

        [Given(@"Shipment Details in body")]
        public void GivenShipmentDetailsInBody()
        {
            var shipment = RequestDataGenerator.GenerateOrderShipmentDetails();

            SetJsonBody(shipment);
        }

        [Given(@"Payment Details in body")]
        public void GivenPaymentDetailsInBody()
        {
            var payment = RequestDataGenerator.GenerateOrderPaymentDetails();

            SetJsonBody(payment);
        }

        [Then(@"response contains order with items")]
        public async Task ThenResponseContainsOrderWithItems()
        {
            var order = await GetResponseJsonData<Order>();

            order.Should().NotBeNull();
            order.Products.Should().NotBeEmpty();
        }

        [Then(@"response contains Shipment Details")]
        public async Task ThenResponseContainsShipmentDetails()
        {
            var shipment = await GetResponseJsonData<Order.ShipmentDetails>();

            shipment.Should().NotBeNull();
        }


        [Then(@"reponse contains Payment Details")]
        public async Task ThenReponseContainsPaymentDetails()
        {
            var payment = await GetResponseJsonData<Order.PaymentDetails>();

            payment.Should().NotBeNull();
        }
    }
}