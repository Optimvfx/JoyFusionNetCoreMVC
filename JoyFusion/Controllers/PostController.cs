using AutoMapper;
using BLL.Models.Post;
using BLL.Models.Post.ViewModels;
using CCL.Base;
using CCL.ControllersLogic;
using JoyFusion.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JoyFusion.Controllers;

[Route("posts")]
public class PostController : BaseAuthController
{
    private readonly PostControllerLogic _logic;
    private readonly ReactionsControllerLogic _reactionsLogic;
    private readonly IMapper _mapper;

    public PostController(PostControllerLogic logic, ReactionsControllerLogic reactionsLogic, IMapper mapper)
    {
        _logic = logic;
        _reactionsLogic = reactionsLogic;
        _mapper = mapper;
    }

    #region Pages

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    #endregion
    
    #region PostsPageViews

    [HttpGet("my/{page=0}")]
    [Authorize]
    public async Task<IActionResult> GetMyPosts(int page)
    {
        var result = await _logic.TryGetUserPostsTopByPublishDate(page, GetUserId().Value);
        if (result.IsSuccess())
            return View("MyPosts", new PostsViewModel(result.Value, page, await _logic.GetPagesCount(GetUserId().Value)));

        return RedirectDefaultUrl();
    }

    [HttpGet("top/popular/{page=0}")]
    public async Task<IActionResult> GetPopular(int page)
    {
        var result = await _logic.TryGetTopByLikes(page);
        if (result.IsSuccess())
            return View("TopPostsByPopularity", new PostsViewModel(result.Value, page, await _logic.GetPagesCount()));

        return RedirectDefaultUrl();
    }
    
    [HttpGet("top/new/{page=0}")]
    public async Task<IActionResult> GetNew(int page)
    {
        var result = await _logic.TryGetTopByPublishDate(page);
        if (result.IsSuccess())
            return View("TopPostsByDate", new PostsViewModel(result.Value, page, await _logic.GetPagesCount()));

        return RedirectDefaultUrl();
    }

    #endregion
    
    [HttpGet("{id}")]
    public async Task<ActionResult> GetDetails(Guid id)
    {
        var result = await _logic.TryGetDetailsById(id);
        
        if (result.IsSuccess())
        {
            return View(result.Value);
        }

        return BadRequest();
    }
    
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<Guid>> Create(PostCreateModel model)
    {
        var result = await _logic.TryCreate(GetUserId().Value, model);
        
        if (result.IsSuccess())
        {
            return Created("Id", result.Value);
        }

        return BadRequest();
    }

    #region Api

    [HttpDelete("api/delete")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid id)
    {
        if (await _logic.TryDelete(GetUserId().Value, id))
            return NoContent();

        return BadRequest();
    }
    
    [HttpPost("api/like/add")]
    [Authorize] 
    public async Task<IActionResult> Like(Guid id)
    {
        if (await _logic.PostExist(id) == false)
            return BadRequest("Post not found");
        
        if (await _reactionsLogic.PostIsLikedByUser(GetUserId().Value, id))
            return BadRequest("Post already liked");

        await _reactionsLogic.LikePost(GetUserId().Value, id);

        return Ok();
    }
    
    [HttpPost("api/like/remove")]
    [Authorize] 
    public  async Task<IActionResult> RemoveLike(Guid id)
    {
        if (await _logic.PostExist(id) == false)
            return BadRequest("Post not found");
            
        if (await  _reactionsLogic.PostIsLikedByUser(GetUserId().Value, id) == false)
            return BadRequest("Post not liked by this user");

        await  _reactionsLogic.RemoveLikeFromPost(GetUserId().Value, id);

        return Ok();
    }

    #endregion
}
