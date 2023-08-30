using System.Security.Claims;
using BLL.Models.Auth;
using BLL.Models.Auth.Requests;
using BLL.Services;
using CCL.Consts;
using Common.Extensions;
using DAL.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace CCL.ControllersLogic;

public class AuthControllerLogic
{
    private readonly UserService _userService;

    public AuthControllerLogic(UserService userService)
    {
        _userService = userService;
    }

    #region Logic
    
    public async Task<bool> TryLogin(CredentialModel model, HttpContext context)
    {
        if (await _userService.UserExistsByCredentials(model) == false)
            return false;
        
        User user = await _userService.GetUserByCredentials( model);
        await Authenticate(user, context);
        return true;
    }
    
    public async Task<bool> TryRegister(RegisterModel model, HttpContext context)
    {
        var userId = await _userService.RegisterUser(model);
        User user = await _userService.GetUserById(userId);
        
        await Authenticate(user, context);

        return true;
    }
    
    public async Task<bool> TryDeleteAccount(Guid userId)
    {
        if (await _userService.UserExistById(userId) == false)
            return false;
        
        await _userService.DeleteUser(userId);
        return true;
    }
    
    public async Task<bool> TryLogout(HttpContext context)
    {
        await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return true;
    }

    #endregion

        
    public async Task<ModelStateErrorsCollection> GenerateRegisterModelStateErrors(RegisterModel model)
    {
        var errors = new ModelStateErrorsCollection();
        
        if (await _userService.CheckUserExistByEmail(model.Email))
            errors.Add(nameof(DAL.Entities.User.Email), "Пользователь с такой почтой уже существует");
        if (await _userService.CheckUserExistByNick(model.Nick))
            errors.Add(nameof(DAL.Entities.User.Nick), "Пользователь с таким ником уже существует");

        return errors;
    }

    public async Task<ModelStateErrorsCollection> GenerateLoginModelStateErrors(CredentialModel model)
    {
        var errors = new ModelStateErrorsCollection();
        
        if(await _userService.UserExistsByCredentials(model) == false)
            errors.Add(nameof(User.Email), "Некорректные логин и(или) пароль");

        return errors;
    }
    
    private async Task Authenticate(User user, HttpContext context)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimsIdentity.DefaultNameClaimType, user.Nick),
            new Claim(Claims.UserClaim, user.Id.ToString()),
            new Claim(ClaimsIdentity.DefaultRoleClaimType, user.RoleId.ToString()),
            new Claim(Claims.EmailClaim, user.Email)
        };

        ClaimsIdentity identity = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType,
            ClaimsIdentity.DefaultRoleClaimType);

        await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
    }
}
