using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MicroAC.Core.Auth;
using MicroAC.Core.Exceptions;
using MicroAC.Core.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Example.ResourceApi
{
    [ApiController]
    public class ResourceApiController : ControllerBase
    {
        readonly IJwtTokenHandler<AccessInternal> _accessInternalTokenHandler;

        public ResourceApiController(IJwtTokenHandler<AccessInternal> accessInternalTokenHandler, IConfiguration config)
        {
            _accessInternalTokenHandler = accessInternalTokenHandler;
        }

        [HttpGet("/Action")]
        [MicroAuth(
            ServiceName = "Service5_SeedTestData",
            Action = "Action742_SeedTestData", 
            Value = "PermissionValue742_SeedTestData"
            )]
        public async Task<ActionResult> Index()
        {
            var response = new
            {
                message = "Hello World! 1s long response.",
                permissions = this.HttpContext.Items[MicroAuthAttribute.PermissionsKey]
            };

            await Task.Delay(1000);

            return Ok(response);
        }

        // TODO: Move Extraction and validation of token to common code library
        private IEnumerable<Permission> Authorize()
        {
            var hasToken = this.Request.Headers.TryGetValue("MicroAC-JWT", out var headerValues);
            var token = headerValues.FirstOrDefault();

            if (!hasToken || token is null)
            {
                throw new AuthenticationFailedException("Missing internal access token.");
            }
            return _accessInternalTokenHandler.GetValidatedPermissions(token);
        }
    }
}
