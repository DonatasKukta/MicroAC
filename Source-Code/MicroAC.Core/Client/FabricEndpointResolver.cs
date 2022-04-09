using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

using Microsoft.ServiceFabric.Services.Client;

using Newtonsoft.Json.Linq;

namespace MicroAC.Core.Client
{
    // TODO: Provide cached version of endpoint retrieved list
    public class FabricEndpointResolver : IEndpointResolver
    {
        readonly ServicePartitionResolver Resolver = ServicePartitionResolver.GetDefault();

        readonly ServicePartitionKey PartitionKey = new ServicePartitionKey();

        readonly CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();

        readonly Dictionary<MicroACServices, string> Services = new Dictionary<MicroACServices, string>()
        {
            { MicroACServices.ResourceApi,    "Example.ResourceApi" },
            { MicroACServices.RequestManager, "MicroAC.RequestManager" },
            { MicroACServices.Authentication, "MicroAC.Authentication" },
            { MicroACServices.Authorization,  "MicroAC.Authorization" },
            { MicroACServices.Orders,         "WebShop.Orders" },
            { MicroACServices.Shipments,      "WebShop.Shipments" },
            { MicroACServices.Cart,           "WebShop.Cart" },
            { MicroACServices.Products,       "WebShop.Products" }
        };                    

        public FabricEndpointResolver()
        {

        }

        public Task<string> GetServiceEndpoint(MicroACServices service) =>
            GetServiceEndpoint($"MicroAC.ServiceFabric/{Services[service]}");

        public async Task<string> GetServiceEndpoint(string fabricServiceType)
        { 
            var uri = new Uri("fabric:/" + fabricServiceType);

            var partition = await Resolver.ResolveAsync( uri, PartitionKey, CancellationTokenSource.Token);

            var addressString = partition.Endpoints.First().Address;
            var addresses = JObject.Parse(addressString);
            var address = (string)addresses["Endpoints"].First();

            return address;
        }
    }
}
