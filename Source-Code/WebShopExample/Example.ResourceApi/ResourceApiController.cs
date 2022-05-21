using System;
using System.Linq;
using System.Threading.Tasks;

using MicroAC.Core.Client;
using MicroAC.Core.Common;

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
        [MicroAuth]
        public ActionResult Index()
        {
            var response = new
            {
                message = "Validated internal access token",
                jwt = this.HttpContext.GetValidatedInternalAccessToken()
            };

            return Ok(response);
        }
    }
}
