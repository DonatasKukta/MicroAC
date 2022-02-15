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

        readonly ITimestampHandler _timestampHandler;

        public ResourceApiController(
            IJwtTokenHandler<AccessInternal> accessInternalTokenHandler, 
            ITimestampHandler timestampHandler)
        {
            _accessInternalTokenHandler = accessInternalTokenHandler;
            _timestampHandler = timestampHandler;
        }

        [HttpGet("/Action")]
        public async Task<ActionResult> Index()
        {
            // TODO: Move Extraction and validation of token to common code
            var hasToken = this.Request.Headers.TryGetValue("MicroAC-JWT", out var headerValues);
            if (!hasToken)
            {
                return UnauthorizedWithTimestamp("Missing internal access token.");
            }

            var token = headerValues.FirstOrDefault();

            if (token is null)
            {
                return UnauthorizedWithTimestamp("Missing internal access token.");
            }

            var permissions = _accessInternalTokenHandler.GetValidatedPermissions(token);
           
            await Task.Delay(1000);
            var response = new
            {
                message = "Hello World! 1s long response.",
                permissions
            };

            return Ok(response);
        }

        private ActionResult UnauthorizedWithTimestamp(string reason)
        {
            _timestampHandler.AddActionMessage("Unauthorized");
            return Unauthorized(reason);
        }
    }
}
