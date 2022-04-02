using System;
using System.Threading.Tasks;
using System.Linq;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System.Net.Http.Headers;
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

    //TODO: This is basically debug logging implementation. Needs refactor (with DI).
    public static class HttpContextTimestampExtensions
    {
        static string _timeNow
        { 
            get { return DateTime.Now.ToString(Constants.TimestampFormat); }
        }

        static string _timestampHeader = Core.Constants.HttpHeaders.Timestamps;

        internal static void AddStartTimestamp(this HttpContext context, string name)
        {
            context.Response.Headers.Append(_timestampHeader, $"{name}-Start-{_timeNow}");
        }

        internal static void AddEndTimestamp(this HttpContext context, string name)
        {
            context.Response.Headers.Append(_timestampHeader, $"{name}-End-{_timeNow}");
        }

        public static void AddActionMessage(this HttpContext context, string name, string message)
        {
            context.Response.Headers.Append(_timestampHeader, $"{name}-{message}-{_timeNow}");
        }

        public static void AppendTimestampHeaders(this HttpContext context, HttpResponseHeaders headers)
        {
            var containsTimestampHeaders = headers.TryGetValues(_timestampHeader, out var timestamps);

            if (containsTimestampHeaders)
            {
                context.Response.Headers.Append(_timestampHeader, new StringValues(timestamps.ToArray()));
            }
        }
    }
}
