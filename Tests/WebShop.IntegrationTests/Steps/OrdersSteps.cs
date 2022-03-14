using System;

using TechTalk.SpecFlow;

namespace WebShop.IntegrationTests.Steps
{
    [Binding]
    [Scope(Feature = "Orders")]
    public sealed class OrdersSteps : SharedSteps
    {
        public OrdersSteps()
            : base(new Uri("http://localhost:19083/MicroAC.ServiceFabric/WebShop.Orders/"))
        {

        }
    }
}