using CCL.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JoyFusionAPI.Controllers.API;

[ApiController]
[Route("api/posts")]
public class UserApiController : BaseAuthController
{
    private readonly UserControllerLogic _logic;

    public UserApiController(UserControllerLogic logic)
    {
        _logic = logic;
    }

    [HttpGet("{user_id}")]
    public IActionResult GetUserInfo(Guid id)
    {
       
    }
    
    [Authorize]
    public IActionResult UpdateUserProfile(UserProfileUpdateRequest request)
    {
        
    }
}