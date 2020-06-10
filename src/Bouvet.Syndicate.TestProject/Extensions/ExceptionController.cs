using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Bouvet.Syndicate.TestProject.Extensions
{
    [ApiController]
    public class ErrorController : ControllerBase
    {
        [Route("/error")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Error() =>
            Problem(title: HttpContext.Features.Get<IExceptionHandlerFeature>().Error.Message);
    }
}