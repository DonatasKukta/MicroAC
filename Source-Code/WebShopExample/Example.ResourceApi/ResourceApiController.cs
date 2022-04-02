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
        public ResourceApiController() 
        { 

        }

        [HttpGet("/Action")]
        [MicroAuth(
            ServiceName = "Service5_SeedTestData",
            Action = "Action742_SeedTestData", 
            Value = "PermissionValue742_SeedTestData"
            )]
        public async Task<ActionResult> Index()
        {
            var resolver = new FabricServiceResolver();

            var resolved = await resolver.GetServiceEndpoints();

            var response = new
            {
                message = "Hello World! 1s long response.",
                permissions = this.HttpContext.Items[HttpContextKeys.Permissions],
                resolved = resolved
            };

            await Task.Delay(1000);

            return Ok(response);
        }
    }
}
