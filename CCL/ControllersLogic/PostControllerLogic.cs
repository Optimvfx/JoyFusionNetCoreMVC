using System.Drawing;
using AutoMapper;
using BLL.Models.Post;
using BLL.Models.Post.ViewModels;
using BLL.Services;
using BLL.Services.ImageService;
using CCL.ControllersLogic.Config;
using Common.Models;
using DAL.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CCL.ControllersLogic;

public class PostControllerLogic
{
    private readonly PostService _postService;
    private readonly IImageService _imageService;
    
    private readonly IMapper _mapper;
    private readonly PostControllersLogicConfig _config;

    private int _postsPerPage => _config.PostsPerPage;
    private WidthHeightRange _widthHeightRange => _config.WidthHeightImageRange;

    public PostControllerLogic(PostService postService, IMapper mapper, PostControllersLogicConfig config, IImageService imageService)
    {
        if (config.PostsPerPage <= 0)
            throw new AggregateException();
        
        _postService = postService;
        _mapper = mapper;
        _config = config;
        _imageService = imageService;
    }

    public async Task<Result<IEnumerable<PostViewModel>>> TryGetTopByLikes(int page)
    {
        return await GetPostTopPage(PostService.PostOrderType.ByLikes, page);
    }
    
    public async Task<Result<IEnumerable<PostViewModel>>> TryGetTopByPublishDate(int page)
    {
        return await GetPostTopPage(PostService.PostOrderType.ByDate, page);
    }

    public async Task<Result<IEnumerable<PostViewModel>>> TryGetUserPostsTopByLikes(int page, Guid userId)
    {
        return await TryGetUserPosts(PostService.PostOrderType.ByLikes, userId, page);
    }
    
    public async Task<Result<IEnumerable<PostViewModel>>> TryGetUserPostsTopByPublishDate(int page, Guid userId)
    {
        return await TryGetUserPosts(PostService.PostOrderType.ByDate, userId, page);
    }
    
    public async Task<Result<PostViewModel>> TryGetById(Guid id)
    {
        if (await _postService.PostExist(id))
        {
            var post = _postService.GetById(id).Include(p => p.Images).First();

            var vm = _mapper.Map<PostViewModel>(post);
            
            return new(ResultStatusCode.Success, vm);
        }

        return new(ResultStatusCode.Failure);
    }
    
    public async Task<Result<PostDetalizedViewModel>> TryGetDetailsById(Guid id)
    {
        if (await _postService.PostExist(id))
        {
            var post = _postService.GetById(id)
                .Include(p => p.Images)
                .Include(p => p.Comments)
                .ThenInclude(c => c.Author)
                .First();

            var vm = _mapper.Map<PostDetalizedViewModel>(post);

            vm.Commens = post.Comments.Select(c => _mapper.Map<CommentViewModel>(c)).ToList();
            
            return new(ResultStatusCode.Success, vm);
        }

        return new(ResultStatusCode.Failure);
    }

    public async Task<Result<Guid>> TryCreate(Guid authorId, PostCreateModel model)
    {
        if (model.Images.Any(i => ImageIsValid(i) == false))
            return new(ResultStatusCode.Failure);

        var newPostId = await _postService.CreatePost(authorId, model);
        return new(ResultStatusCode.Success, newPostId);
    }

    public async Task<bool> TryDelete(Guid authorId, Guid id)
    {
        if (await _postService.PostExist(authorId, id) == false)
            return false;

        await _postService.DeletePost(authorId, id);
        return true;
    }

    public async Task<bool> PostExist(Guid authorId, Guid id)
    {
        return await _postService.PostExist(authorId, id);
    }
    
    public async Task<bool> PostExist(Guid id)
    {
        return await _postService.PostExist(id);
    }

    public async Task<int> GetPagesCount(Guid? userId = null)
    {
        const int MinimalPostsPerPage = 1;
        
        var postsCount = await _postService.GetPostsCount(userId);

        return Math.Max(postsCount - MinimalPostsPerPage, 0) / _postsPerPage;
    }

    private bool ImageIsValid(Image image)
    {
        if (image == null)
            return false;
        
        return _widthHeightRange.InRange(image);
    }
    
    private async Task<Result<IEnumerable<PostViewModel>>> GetPostTopPage(PostService.PostOrderType postOrderType, int page)
    {
        if (await PageIsValid(page) == false)
            return new(ResultStatusCode.Failure);

        
        return new(ResultStatusCode.Success, 
            MapPosts(_postService.GetPostTopByOrderType(postOrderType,
                    GetSkipCount(page), _postsPerPage)));
    }

    private async Task<Result<IEnumerable<PostViewModel>>> TryGetUserPosts(PostService.PostOrderType postOrderType, Guid userId, int page)
    {
        if (await PageIsValid(page, userId) == false)
            return new(ResultStatusCode.Failure);

        return new(ResultStatusCode.Success,
            MapPosts(_postService.GetUserPosts(postOrderType, userId, GetSkipCount(page), _postsPerPage)));
    }
    
    private IEnumerable<PostViewModel> MapPosts(IEnumerable<Post> posts)
    {
        return posts.Select(p => _mapper.Map<PostViewModel>(p));
    }

    private async Task<bool> PageIsValid(int page, Guid? userId = null)
    {
        var postsCount = _postService.GetPostsCount();

        return page >= 0 && page <= await GetPagesCount(userId);
    }
   
    private int GetSkipCount(int page)
    {
        return  page * _postsPerPage;
    }
}
