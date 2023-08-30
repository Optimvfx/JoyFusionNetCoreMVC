using System.Net;
using Common.Exceptions.General;
using Common.Extensions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Controller = Microsoft.AspNetCore.Mvc.Controller;

namespace CCL.Base;

public abstract class BaseController : Controller
{
    public RedirectToActionResult CustomRedirectUrl { get; set; }

    protected IActionResult RedirectToUrl(string? returnUrl)
    {
        if (returnUrl == null)
            return RedirectDefaultUrl();
        
        return Redirect(returnUrl);
    }

    protected IActionResult RedirectToTempDataReturnUrl()
    {
        var url = TempData.GetReturnUrl();

        if (url == null)
            return RedirectDefaultUrl();
        
        return Redirect(url);
    }

    protected IActionResult RedirectDefaultUrl()
    {
        if (CustomRedirectUrl == null)
            return Redirect("/Index/Home");

        return CustomRedirectUrl;
    }
    
    protected BadRequestObjectResult BadRequest(ModelStateErrorsCollection modelStateError)
    {
        if (modelStateError.HaveAnyError == false)
            throw new ArgumentException();
        
        var errors = modelStateError.Errors;
        
        string errorMessage = JsonConvert.SerializeObject(errors);


        return BadRequest(errorMessage);
    }
}