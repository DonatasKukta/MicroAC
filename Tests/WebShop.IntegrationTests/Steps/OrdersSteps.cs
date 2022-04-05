using System;
using System.Threading.Tasks;

using FluentAssertions;

 using MicroAC.Core.Client;

using TechTalk.SpecFlow;

using WebShop.Common;

namespace WebShop.IntegrationTests.Steps
{
    [Binding]
    [Scope(Feature = "Orders")]
    public sealed class OrdersSteps : SharedSteps
    {
        public OrdersSteps() : base(MicroACServices.Orders)
        {

        }

        [Given(@"(.*) in path")]
        public void GivenGuidInPath(string pathParameter)
        {
            if(pathParameter == "Guid")
                this.AppendToRequestUrl(Guid.NewGuid().ToString());
            else
                this.AppendToRequestUrl(pathParameter);
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
            var shipment = await GetResponseJsonData<Shipment>();

            shipment.Should().NotBeNull();
        }


        [Then(@"response contains Payment Details")]
        public async Task ThenResponseContainsPaymentDetails()
        {
            var payment = await GetResponseJsonData<Order.PaymentDetails>();

            payment.Should().NotBeNull();
        }
    }
}