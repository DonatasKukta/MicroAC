using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Fabric;

namespace MicroAC.Core.Common
{
    public class TimestampMiddleware
    {
        readonly RequestDelegate _next;

        readonly string _serviceName;

        readonly StatelessServiceContext _serviceContext;

        public TimestampMiddleware(
            RequestDelegate next, 
            IConfiguration config, 
            StatelessServiceContext serviceContext)
        {
            _next = next;
            _serviceName = config.GetSection("Timestamp:ServiceName").Value;
            _serviceContext = serviceContext;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            context.AddActionMessage(_serviceName, _serviceContext.NodeContext.NodeName);
            context.AddStartTimestamp(_serviceName);
            context.Response.OnStarting(state =>
            {
                (state as HttpContext).AddEndTimestamp(_serviceName);
                return Task.CompletedTask;
            }, context);

            await _next(context);
        }
    }
}
