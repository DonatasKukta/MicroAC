using System;
using System.Threading;
using System.Linq;
using System.Text.Json;

using Microsoft.ServiceFabric.Services.Client;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace MicroAC.Core.Client
{
    public class FabricServiceResolver
    {
        readonly ServicePartitionResolver Resolver = ServicePartitionResolver.GetDefault();

        readonly ServicePartitionKey PartitionKey = new ServicePartitionKey();

        readonly CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();

        public FabricServiceResolver()
        {

        }

        // TODO: Provide a list of addresses.
        public async Task<string> GetServiceEndpoint()
        {
            var uri = new Uri(@"fabric:/MicroAC.ServiceFabric/MicroAC.Authorization");

            var partition =
                await Resolver.ResolveAsync(
                    uri,
                    PartitionKey,
                    CancellationTokenSource.Token);

            var addressString = partition.Endpoints.First().Address;
            var addresses = JObject.Parse(addressString);
            var address = (string)addresses["Endpoints"].First();

            return address;
        }
    }
}
