using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace OpsPilot.Web.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
public class ErrorController : Controller
{
    [Route("/Error")]
    public IActionResult Error()
    {
        var exceptionFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
        ViewBag.ErrorPath = exceptionFeature?.Path;
        return View("~/Views/Shared/Error.cshtml");
    }
}
