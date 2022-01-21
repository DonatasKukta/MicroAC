using System;
using System.Threading.Tasks;
using System.Linq;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System.Net.Http.Headers;
using System.Web.Mvc;

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

    //TODO: This is basically debug logging implementation. Needs refactor (with DI).
    public static class HttpContextTimestampExtensions
    {
        static string _timeNow
        { 
            get { return DateTime.Now.ToString("yyyy/MM/ddThh:mm:ss.fff+02:00"); }
        }

        internal static void AddStartTimestamp(this HttpContext context, string header, string name)
        {
            context.Response.Headers.Append(header, $"{name}-Start-{_timeNow}");
        }

        internal static void AddEndTimestamp(this HttpContext context, string header, string name)
        {
            context.Response.Headers.Append(header, $"{name}-End-{_timeNow}");
        }

        public static void AddActionMessage(this HttpContext context, string header, string name, string message)
        {
            context.Response.Headers.Append(header, $"{name}-{message}-{_timeNow}");
        }

        public static void AppendeTimestampHeaders(this HttpContext context, string key, HttpResponseHeaders headers)
        {
            var containsTimestampHeaders = headers.TryGetValues(key, out var timestamps);

            if (containsTimestampHeaders)
            {
                context.Response.Headers.Append(key, new StringValues(timestamps.ToArray()));
            }
        }
    }
}
