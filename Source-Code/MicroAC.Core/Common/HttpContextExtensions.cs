using System;
using System.Linq;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System.Net.Http.Headers;
using MicroAC.Core.Constants;
using MicroAC.Core.Models;
using System.Collections.Generic;

namespace MicroAC.Core.Common
{

    public static class HttpContextExtensions
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

        public static IEnumerable<Permission> GetValidatedPermissions(this HttpContext context)
        {
            return context.Items[HttpContextKeys.Permissions] as IEnumerable<Permission>;
        }

        public static string GetValidatedInternalAccessToken(this HttpContext context)
        {
            return context.Items[HttpContextKeys.InternalAccessToken] as string;
        }

        public static IEnumerable<string> GetValidatedRoles(this HttpContext context)
        {
            return context.Items[HttpContextKeys.Roles] as IEnumerable<string>;
        }
    }
}
