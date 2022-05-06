using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Linq;

using Microsoft.Extensions.Configuration;
using Microsoft.ServiceFabric.Services.Client;

using Newtonsoft.Json.Linq;
using System.Fabric;
using MicroAC.Core.Constants;

namespace MicroAC.Core.Client
{
    public class FabricEndpointResolver : IEndpointResolver
    {
        bool Initialised = false;

        readonly object InitialisationLock = new object();

        readonly ServicePartitionResolver Resolver;// = ServicePartitionResolver.GetDefault();

        readonly ServicePartitionKey PartitionKey = new ServicePartitionKey();

        readonly CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();

        readonly ConcurrentDictionary<string, EndpointContainer> ResolvedEndpoints = new();

        static string GetFabricType(MicroACServices service) 
            => $"fabric:/MicroAC.ServiceFabric/{Fabric.GetServiceTypeName(service)}";
        static string GetFabricType(string service) 
            => $"fabric:/{service}";

        public FabricEndpointResolver(IConfiguration config) 
            : this(config.GetValue<string>(ConfigKeys.SfClusterClientConnectionEndpoint))
        {

        }

        public FabricEndpointResolver(string endpoint)
        {
            Resolver = new ServicePartitionResolver(endpoint);
        }

        public void InitialiseEndpoints()
        {
            lock (InitialisationLock)
            {
                if (Initialised)
                {
                    return;
                }

                foreach (var service in Fabric.GetRegisteredServices())
                {
                    var fabricType = GetFabricType(service);
                    var partition = Resolver.ResolveAsync(new Uri(fabricType), PartitionKey, CancellationTokenSource.Token).Result;
                    var endpoints = partition.Endpoints.Select(endpoint => GetEndpoint(endpoint)).ToList();
                    var container = new EndpointContainer(endpoints);
                    if (!ResolvedEndpoints.TryAdd(fabricType, container))
                        throw new Exception($"Unable to add EndPoint container of '{fabricType}'");
                }
                Initialised = true;
            }
        }

        public string GetServiceEndpoint(MicroACServices service) 
            => GetEndpoint(GetFabricType(service));

        public string GetServiceEndpoint(string fabricService)
            => GetEndpoint(GetFabricType(fabricService));

        string GetEndpoint(string fabricType)
        {
            var container = ResolvedEndpoints.GetValueOrDefault(fabricType);
            return container?.GetEndpoint();
        }

        string GetEndpoint(ResolvedServiceEndpoint endpoint)
        {
            var address = JObject.Parse(endpoint.Address);
            return (string)address["Endpoints"].First();
        }
    }
}
