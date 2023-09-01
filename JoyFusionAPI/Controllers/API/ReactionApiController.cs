using System.ComponentModel.DataAnnotations;
using AutoMapper;
using CCL.Base;
using CCL.ControllersLogic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JoyFusionAPI.Controllers.API;

[ApiController]
[Route("api/reaction")]
public class ReactionApiController : BaseAuthController
{
    private readonly ReactionsControllerLogic _logic;
    private readonly PostControllerLogic _postControllersLogic;
    private readonly IMapper _mapper;

    public ReactionApiController(ReactionsControllerLogic logic, PostControllerLogic postControllersLogic, IMapper mapper)
    {
        _logic = logic;
        _postControllersLogic = postControllersLogic;
        _mapper = mapper;
    }

    [Display(Name = "Add Like On Post")]
    [HttpPost("{id}/like/add", Name = "Add Like")]
    [Authorize] 
    public async Task<IActionResult> Like(Guid id)
    {
        if (await _postControllersLogic.PostExist(id) == false)
            return BadRequest("Post not found");
        
        if (await _logic.PostIsLikedByUser(GetUserId().Value, id))
            return BadRequest("Post already liked");

        await _logic.LikePost(GetUserId().Value, id);

        return Ok();
    }
    
    [Display(Name = "Remove Like On Post")]
    [HttpPost("{id}/like/remove", Name = "Remove Like")]
    [Authorize] 
    public  async Task<IActionResult> RemoveLike(Guid id)
    {
        if (await _postControllersLogic.PostExist(id) == false)
            return BadRequest("Post not found");
            
        if (await _logic.PostIsLikedByUser(GetUserId().Value, id) == false)
            return BadRequest("Post not liked by this user");

        await _logic.RemoveLikeFromPost(GetUserId().Value, id);

        return Ok();
    }

    [Display(Name = "Post Have Like By Me")]
    [HttpGet("{id}/like/have", Name = "Have Like")]
    [Authorize] 
    public async Task<ActionResult<bool>> PostHaveLikeFromThisUser(Guid id)
    {
        if (await _postControllersLogic.PostExist(id)  == false)
            return BadRequest("Post not found");

        return Ok(await _logic.PostIsLikedByUser(GetUserId().Value, id));
    }
}
