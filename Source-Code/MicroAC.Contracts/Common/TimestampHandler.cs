using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace MicroAC.Core.Common
{
    /*
    The purpose of TimestampHandler is to handle timestamp appending to Http Response Headers.
    This implementation is supposed to be a replacement for extension methods, however
    due to non-thread safe and request-lifecycle dependant nature of HttpContext,
    it's injection into such service could be done only through IHttpContextAccesor interface.
    Using IHttpContextAccesor would introduce non-trivial perfromance costs, therefore implementation
    of this class is temporarily stubbed-out.
    https://github.com/aspnet/Announcements/issues/190#issue-159298966
    TODO: Implement timestamp appending to HttpContext
     */

    public class TimestampHandler : ITimestampHandler
    {
        readonly string _header;
        readonly string _name;
        readonly bool _enabled;

        string _timeNow
        {
            get { return DateTime.Now.ToString(Constants.TimestampFormat); }
        }

        public TimestampHandler(IConfiguration config)
        {
            _header = config.GetSection("Timestamp:Header").Value;
            _name = config.GetSection("Timestamp:ServiceName").Value;
            _enabled = bool.Parse(config.GetSection("Timestamp:Enabled").Value);
        }

        void ITimestampHandler.AddStartTimestamp() => AddActionMessage("Start");
        void ITimestampHandler.AddEndTimestamp() => AddActionMessage("End");

        public void AddActionMessage(string message)
        {
            if (!_enabled) return;

            //IHttpContextAccessor.HttpContext.Response.Headers.Append(_header, $"{_name}-{message}-{_timeNow}");
        }

        public void AppendeTimestampHeaders(HttpResponseHeaders headers)
        {
            if (_enabled && headers.TryGetValues(_header, out var timestamps))
            {
                //IHttpContextAccessor.HttpContext.Response.Headers.Append(_header, new StringValues(timestamps.ToArray()));
            }
        }
    }
}
