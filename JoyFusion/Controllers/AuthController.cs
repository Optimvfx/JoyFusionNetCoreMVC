using AutoMapper;
using BLL.Models.Auth;
using BLL.Models.Auth.Requests;
using CCL.Base;
using CCL.ControllersLogic;
using Common.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JoyFusion.Controllers;

public class AuthController : BaseAuthController
{
    private readonly IMapper _mapper;
    private readonly AuthControllerLogic _logic;

    public AuthController(IMapper mapper, AuthControllerLogic logic)
    {
        _mapper = mapper;
        _logic = logic;
    }

    #region Pages

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Register(string? returnUrl = null)
    {
        if (UserAlreadyAuthorized())
            return RedirectToUrl(returnUrl);
        
        if (returnUrl != null) TempData.SetReturnUrl(returnUrl);
        return View();
    }
    
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        var id = GetUserId();
        
        if (UserAlreadyAuthorized())
            return RedirectToUrl(returnUrl);
        
        if (returnUrl != null) TempData.SetReturnUrl(returnUrl);
        return View();
    }

    #endregion
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        if (UserAlreadyAuthorized())
            return RedirectToTempDataReturnUrl();

        if (!ModelState.IsValid)
            return View(request);

        var model = _mapper.Map<CredentialModel>(request);
        var modelErrors = await _logic.GenerateLoginModelStateErrors(model);

        if (modelErrors.HaveAnyError)
        {
            ModelState.AddModelErrors(modelErrors);
            return View(request);
        }

        if (await _logic.TryLogin(model, HttpContext) == false)
            return View(request);

        return RedirectToTempDataReturnUrl();
    }
    

    [HttpPost]
    [ValidateAntiForgeryToken]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        if (UserAlreadyAuthorized())
            return RedirectToTempDataReturnUrl();
        
        if (!ModelState.IsValid)
            return View(request);

        var model = _mapper.Map<RegisterModel>(request);
        var modelErrors = await _logic.GenerateRegisterModelStateErrors(model);
        
        if(modelErrors.HaveAnyError)
        {
            ModelState.AddModelErrors(modelErrors);
            return View("Register", request);
        }
        
        if (await _logic.TryRegister(model, HttpContext) == false)
            return View("Register", request);
        
        return RedirectToTempDataReturnUrl();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> DeleteAccount(string? returnUrl = null)
    {
        if (await _logic.TryDeleteAccount(GetUserId().Value) == false)
            return RedirectDefaultUrl();
        
        return await Logout(returnUrl);
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Logout(string? returnUrl = null)
    {
        await _logic.TryLogout(HttpContext);
        
        return RedirectToUrl(returnUrl);
    }
}
