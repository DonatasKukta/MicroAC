using System;
using System.Linq;
using System.Threading.Tasks;

using MicroAC.Core.Client;
using MicroAC.Core.Constants;

using Microsoft.AspNetCore.Mvc;

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
            var resolver = new FabricEndpointResolver();

            var services = Enum.GetValues(typeof(MicroACServices))
                .Cast<MicroACServices>()
                .Select(s =>  resolver.GetServiceEndpoint(s).Result);

            var response = new
            {
                message = "Hello World! 1s long response.",
                permissions = this.HttpContext.Items[HttpContextKeys.Permissions],
                resolved = services
            };

            await Task.Delay(100);

            return Ok(response);
        }
    }
}
