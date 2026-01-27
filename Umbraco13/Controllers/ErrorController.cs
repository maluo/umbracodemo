using Microsoft.AspNetCore.Mvc;

namespace Umbraco13.Controllers;

public class ErrorController : Umbraco.Cms.Web.Common.Controllers.UmbracoController
{
    public IActionResult Index(int statusCode)
    {
        return statusCode switch
        {
            400 => View("BadRequest"),
            401 => View("Unauthorized"),
            403 => View("Forbidden"),
            404 => View("NotFound"),
            500 => View("InternalServerError"),
            _ => View("Error")
        };
    }

    public IActionResult BadRequest()
    {
        Response.StatusCode = 400;
        return View();
    }

    public IActionResult Unauthorized()
    {
        Response.StatusCode = 401;
        return View();
    }

    public IActionResult Forbidden()
    {
        Response.StatusCode = 403;
        return View();
    }

    public IActionResult NotFound()
    {
        Response.StatusCode = 404;
        return View();
    }

    public IActionResult InternalServerError()
    {
        Response.StatusCode = 500;
        return View();
    }

    public IActionResult Error()
    {
        Response.StatusCode = 500;
        return View();
    }
}