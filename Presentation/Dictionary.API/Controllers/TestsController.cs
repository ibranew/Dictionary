using Dictionary.API.Controllers.Common;
using Dictionary.Application.Common.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dictionary.API.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Policy = DictionaryPolicies.AdminOnly)]
    [ApiController]
    public class TestsController : DictionaryControllerBase
    {
        [HttpGet("gettest")]
        [Authorize]
        public IActionResult GetTest()
        {
            return Ok(new
            {
                Authenticated = User.Identity?.IsAuthenticated,

                Roles = User.Claims
                    .Where(x =>
                        x.Type.Contains("role") ||
                        x.Type.Contains("Role"))
                    .Select(x => new
                    {
                        x.Type,
                        x.Value
                    }),

                AllClaims = User.Claims.Select(x => new
                {
                    x.Type,
                    x.Value
                })
            });
        }
    }
}
