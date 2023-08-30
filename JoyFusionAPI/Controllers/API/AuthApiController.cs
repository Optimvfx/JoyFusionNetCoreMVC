using System.ComponentModel.DataAnnotations;
using AutoMapper;
using BLL.Models.Auth;
using BLL.Models.Auth.Requests;
using CCL.Base;
using CCL.ControllersLogic;
using Common.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JoyFusionAPI.Controllers.API;

[ApiController]
[Route("api/auth")]
public class AuthApiController : BaseAuthController
{
    private readonly AuthControllerLogic _logic;

    public AuthApiController(AuthControllerLogic logic)
    {
        _logic = logic;
    }

    [Display(Name = "Login")]
    [HttpPost]
    [AllowAnonymous]
    [Route("login", Name = "Login")]
    public async Task<IActionResult> Login(CredentialModel model)
    {
        var errors = await _logic.GenerateLoginModelStateErrors(model);

        if (errors.HaveAnyError)
            return BadRequest(errors);

        if (await _logic.TryLogin(model, HttpContext))
            return Ok();

        return BadRequest();
    }
    
    [Display(Name = "Register")]
    [HttpPost]
    [AllowAnonymous]
    [Route("register", Name = "Register")]
    public async Task<IActionResult> Register(RegisterModel model)
    {
        var errors = await _logic.GenerateRegisterModelStateErrors(model);

        if (errors.HaveAnyError)
            return BadRequest(errors);

        if (await _logic.TryRegister(model, HttpContext))
            return Ok();

        return BadRequest();
    }

    [Display(Name = "Delete Account")]
    [HttpPost("delete", Name = "Delete Account")]
    [Authorize]
    public async Task<IActionResult> DeleteAccount()
    {
        if (await _logic.TryDeleteAccount(GetUserId().Value))
            return await Logout();

        return BadRequest();
    }

    [Display(Name = "Logout")]
    [HttpPost("logout", Name = "Logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        if (await _logic.TryLogout(HttpContext))
            return NoContent();

        return NotFound();
    }
}