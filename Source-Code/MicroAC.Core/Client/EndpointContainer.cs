using System;
using System.Collections.Generic;
using System.Linq;

namespace MicroAC.Core.Client
{
    class EndpointContainer
    {
        readonly object ReadLock = new object();

        readonly List<string> Endpoints;

        int Index;

        int Count;

        public EndpointContainer(List<string> endpoints)
        {

            if (endpoints?.Count < 1)
            {
                throw new Exception("EndpointContainer cannot be initialised with empty list of endpoints.");
            }

            Endpoints = endpoints.ToList();
            Index = 0;
            Count = Endpoints.Count;
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
    }
}
