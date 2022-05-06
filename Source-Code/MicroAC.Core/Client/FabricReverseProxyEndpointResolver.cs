using System.Collections.Generic;
using System.Linq;

using MicroAC.Core.Constants;

using Microsoft.Extensions.Configuration;

namespace MicroAC.Core.Client
{
    public class FabricReverseProxyEndpointResolver : IEndpointResolver
    {
        readonly EndpointContainer ReverseProxyEndpoints;

        public FabricReverseProxyEndpointResolver(IConfiguration config)
            : this(config.GetValue<string>(ConfigKeys.SfReverseProxyIp),
                   config.GetSection(ConfigKeys.SfReverseProxyPorts).Get<int[]>())
        { }

        public FabricReverseProxyEndpointResolver(string reverseProxyIp, IEnumerable<int> reverseProxyPorts)
        {
            var endpoints = reverseProxyPorts
                .Select(port => $"http://{reverseProxyIp}:{port}")
                .ToList();

            ReverseProxyEndpoints = new EndpointContainer(endpoints);
        }

        public void InitialiseEndpoints() { }

        public string GetServiceEndpoint(MicroACServices service) =>
            $"{ReverseProxyEndpoints.GetEndpoint()}/MicroAC.ServiceFabric/{Fabric.GetServiceTypeName(service)}";

        public string GetServiceEndpoint(string fabricService) =>
            $"{ReverseProxyEndpoints.GetEndpoint()}/{fabricService}";
    }
}
