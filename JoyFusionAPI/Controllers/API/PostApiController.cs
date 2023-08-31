using System.ComponentModel.DataAnnotations;
using AutoMapper;
using BLL.Models.Post.Request;
using BLL.Models.Post.ViewModels;
using CCL.Base;
using CCL.ControllersLogic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JoyFusionAPI.Controllers.API;

[ApiController]
[Route("api/posts")]
public class PostApiController : BaseAuthController
{
    private readonly PostControllerLogic _logic;
    private readonly IMapper _mapper;

    public PostApiController(PostControllerLogic logic, IMapper mapper)
    {
        _logic = logic;
        _mapper = mapper;
    }

    [Display(Name = "Get Popular")]
    [Route("popular/{page}", Name = "Get Popular")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PostViewModel>>> GetPopularPosts(int page)
    {
        var result = await _logic.TryGetTopByLikes(page);
        if (result.IsSuccess())
            return Ok(result.Value);

        return BadRequest();
    }
    
    [Display(Name = "Get New")]
    [Route("new/{page}", Name = "Get New")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PostViewModel>>> GetNewPosts(int page)
    {
        var result = await _logic.TryGetTopByPublishDate(page);
        if (result.IsSuccess())
            return Ok(result.Value);

        return BadRequest();
    }

    [Display(Name = "Get My")]
    [Route("my/{page}", Name = "Get My")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PostViewModel>>> GetMyPosts(int page)
    {
        var result = await _logic.TryGetUserPostsTopByPublishDate(page, GetUserId().Value);
        if (result.IsSuccess())
            return Ok(result.Value);

        return BadRequest();
    }

    [Display(Name = "Get My Page Count")]
    [Route("my/page/count", Name = "Get My Page Count")]
    [Authorize]
    [HttpGet]
    public async Task<ActionResult<int>> GetMyPostPagesCount()
    {
        return await _logic.GetPagesCount(GetUserId().Value);
    }
    
    [Display(Name = "Get Page Count")]
    [Route("page/count", Name = "Get Page Count")]
    [HttpGet]
    public async Task<ActionResult<int>> GetPostPagesCount()
    {
        return await _logic.GetPagesCount();
    }
    
    [Display(Name = "Get Post")]
    [HttpGet("{id}", Name = "Get Post")]
    public async Task<ActionResult<PostViewModel>> GetPostDetails(Guid id)
    {
        var result = await _logic.TryGetById(id);
        
        if (result.IsSuccess())
        {
            return _mapper.Map<PostViewModel>(result.Value);
        }

        return BadRequest();
    }

    [Display(Name = "Create Post")]
    [HttpPost("create", Name = "Create Post")]
    [Authorize]
    public async Task<ActionResult<Guid>> CreatePost(PostCreateRequest request)
    {
        var result = await _logic.TryCreate(GetUserId().Value, request);
        
        if (result.IsSuccess())
        {
            return Created("Id", result.Value);
        }

        return BadRequest();
    }

    [Display(Name = "Delete Post")]
    [HttpDelete("{id}", Name = "Delete Post")]
    [Authorize]
    public async Task<IActionResult> DeletePost(Guid id)
    {
        if (await _logic.TryDelete(GetUserId().Value, id))
            return NoContent();

        return BadRequest();
    }

    [Display(Name = "Add Like On Post")]
    [HttpPost("{id}/like/add", Name = "Add Like")]
    [Authorize] 
    public async Task<IActionResult> Like(Guid id)
    {
        if (await _logic.PostExist(id) == false)
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
        if (await _logic.PostExist(id) == false)
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
        if (await _logic.PostExist(id)  == false)
            return BadRequest("Post not found");

        return Ok(await _logic.PostIsLikedByUser(GetUserId().Value, id));
    }
}
