using System.Net.Http.Headers;

using Microsoft.AspNetCore.Http;

namespace MicroAC.Core.Common
{
    public interface ITimestampHandler
    {
        internal void AddStartTimestamp();

        internal void AddEndTimestamp();

        public void AddActionMessage(string message);

        public void AppendeTimestampHeaders(HttpResponseHeaders headers);
    }
}
