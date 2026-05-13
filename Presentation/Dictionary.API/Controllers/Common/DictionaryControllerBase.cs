using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Dictionary.API.Controllers.Common
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class DictionaryControllerBase : ControllerBase
    {
        private ISender? _sender;
        protected ISender Sender =>
            _sender ??= HttpContext.RequestServices.GetRequiredService<ISender>();
    }
}
