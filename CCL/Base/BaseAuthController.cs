using CCL.Consts;
using Common.Convertors;
using Microsoft.AspNetCore.Mvc;

namespace CCL.Base;

public class BaseAuthController : BaseController
{
    protected Guid? GetUserId()
    {
        if (User.Claims.TryGetClaimValue(Claims.UserClaim, out Guid id))
            return id;

        return null;
    }

    protected bool UserAlreadyAuthorized() => GetUserId() != null;
    
    protected IActionResult AlreadyAuthorizedForbid() => Forbid("Already Authorized");
}