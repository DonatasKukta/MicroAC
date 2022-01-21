using System.Linq;
using System.Threading.Tasks;

using MicroAC.Core.Auth;
using MicroAC.Core.Common;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace Example.ResourceApi
{
    [ApiController]
    public class ResourceApiController : ControllerBase
    {
        readonly IJwtTokenHandler<AccessInternal> _accessInternalTokenHandler;

        readonly string _timestampHeader;

        readonly string _serviceName;

        public ResourceApiController(IJwtTokenHandler<AccessInternal> accessInternalTokenHandler, IConfiguration config)
        {
            _accessInternalTokenHandler = accessInternalTokenHandler;
            _timestampHeader = config.GetSection("Timestamp:Header").Value;
            _serviceName = config.GetSection("Timestamp:ServiceName").Value;
        }

        [HttpGet("/Action")]
        public async Task<ActionResult> Index()
        {
            // TODO: Move Extraction and validation of token to common code
            var hasToken = this.Request.Headers.TryGetValue("MicroAC-JWT", out StringValues headerValues);
            if (!hasToken)
            {
                return UnauthorizedWithTimestamp("Missing internal access token.");
            }

            var token = headerValues.FirstOrDefault();

            if (token is null)
            {
                return UnauthorizedWithTimestamp("Missing internal access token.");
            }

            _accessInternalTokenHandler.Validate(token);

            await Task.Delay(1000);
            var response = new
            {
                message = "Hello World! 1s long response.",
                internalAccessToken = token
            };

            return Ok(response);
        }

        private ActionResult UnauthorizedWithTimestamp(string reason)
        {
            this.HttpContext.AddActionMessage(_timestampHeader, _serviceName, "Unauthorized");
            return Unauthorized(reason);
        }
    }
}
