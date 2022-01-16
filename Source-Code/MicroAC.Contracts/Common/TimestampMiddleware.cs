using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace MicroAC.Core.Common
{
    public class TimestampMiddleware
    {
        readonly RequestDelegate _next;

        readonly string _timestampHeader;

        readonly string _serviceName;

        public TimestampMiddleware(RequestDelegate next, IConfiguration config)
        {
            _next = next;
            _timestampHeader = config.GetSection("Timestamp:Header").Value;
            _serviceName = config.GetSection("Timestamp:ServiceName").Value;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            context.AddStartTimestamp(_timestampHeader, _serviceName);
            context.Response.OnStarting(state =>
            {
                (state as HttpContext).AddEndTimestamp(_timestampHeader, _serviceName);
                return Task.CompletedTask;
            }, context);

            await _next(context);
        }
    }

    public static class HttpContextTimestampExtensions
    {
        static string _timestamp
        {
            get { return DateTime.Now.ToString("hh.mm.ss.ffff"); }
        }

        internal static void AddStartTimestamp(this HttpContext context, string header, string name)
        {
            context.Response.Headers.Append(header, $"Start-{name}-{_timestamp}");
        }

        internal static void AddEndTimestamp(this HttpContext context, string header, string name)
        {
            context.Response.Headers.Append(header, $"End-{name}-{_timestamp}");
        }

        public static void AddActionMessage(this HttpContext context, string header, string name, string message)
        {
            context.Response.Headers.Append(header, $"{name}-{message}-{_timestamp}");
        }
    }
}
