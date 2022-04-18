using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Linq;

using Microsoft.Extensions.Configuration;
using Microsoft.ServiceFabric.Services.Client;

using Newtonsoft.Json.Linq;
using System.Fabric;

namespace MicroAC.Core.Client
{
    public class FabricEndpointResolver : IEndpointResolver
    {
        bool Initialised = false;

        readonly object InitialisationLock = new object();

        readonly ServicePartitionResolver Resolver;// = ServicePartitionResolver.GetDefault();

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

        readonly ConcurrentDictionary<string, EndpointContainer> ResolvedEndpoints = new();

        string GetFabricType(MicroACServices service) => $"fabric:/MicroAC.ServiceFabric/{Services[service]}";
        string GetFabricType(string service) => $"fabric:/{service}";

        public FabricEndpointResolver(IConfiguration config) 
            : this(config.GetValue<string>("SfClusterClientConnectionEndpoint"))
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
                if (Initialised) return;

                foreach (var service in Services.Keys)
                {
                    var fabricType = GetFabricType(service);
                    var partition = Resolver.ResolveAsync(new Uri(fabricType), PartitionKey, CancellationTokenSource.Token).Result;
                    var container = new EndpointContainer(partition);
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

        class EndpointContainer
        {
            readonly object ReadLock = new object();

            readonly List<string> Endpoints;

            int Index;

            int Count;

            public EndpointContainer(ResolvedServicePartition partition)
            {
                Endpoints = partition.Endpoints.Select(endpoint => GetURL(endpoint)).ToList();
                Index = 0;
                Count = Endpoints.Count;

                if (Count < 1)
                {
                    throw new Exception("Unable to extract endpoints from ResolvedServicePartition");
                }
            }

            public string GetEndpoint()
            {
                lock (ReadLock)
                {
                    var endpoint = Endpoints[Index];

                    if (Index == Count - 1)
                    {
                        Index = 0;
                    }
                    else
                    {
                        Index++;
                    }

                    return endpoint;
                }
            }

            string GetURL(ResolvedServiceEndpoint endpoint)
            {
                var address = JObject.Parse(endpoint.Address);
                return (string)address["Endpoints"].First();
            }
        }
    }
}
