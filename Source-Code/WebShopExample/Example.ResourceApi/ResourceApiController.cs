using System.Threading.Tasks;

using MicroAC.Core.Auth;
using MicroAC.Core.Client;
using MicroAC.Core.Constants;

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
                permissions = this.HttpContext.Items[HttpContextKeys.Permissions]
            };

            await Task.Delay(1000);

            return Ok(response);
        }
    }
}
