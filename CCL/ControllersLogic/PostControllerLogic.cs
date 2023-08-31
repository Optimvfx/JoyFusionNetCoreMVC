using AutoMapper;
using BLL.Models.Post.Request;
using BLL.Models.Post.ViewModels;
using BLL.Services;
using CCL.ControllersLogic.Config;
using Common.Models;
using DAL.Entities;
using Microsoft.AspNetCore.Mvc;

namespace CCL.ControllersLogic;

public class PostControllerLogic
{
    private readonly PostService _postService;
    private readonly IMapper _mapper;

    private readonly int _postsPerPage;

    public PostControllerLogic(PostService postService, IMapper mapper, PostControllersLogicConfig config)
    {
        if (config.PostsPerPage <= 0)
            throw new AggregateException();
            
        _postService = postService;
        _mapper = mapper;
        _postsPerPage = config.PostsPerPage;
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
    
    public async Task<Result<Post>> TryGetById(Guid id)
    {
        if (await _postService.PostExist(id))
            return new(ResultStatusCode.Success,
                await _postService.GetByIdWithImages(id));

        return new(ResultStatusCode.Failure);
    }

    public async Task<Result<Guid>> TryCreate(Guid authorId, PostCreateRequest request)
    {
        var newPostId = await _postService.CreatePost(authorId, request);
        return new(ResultStatusCode.Success, newPostId);
    }

    public async Task<bool> TryDelete(Guid authorId, Guid id)
    {
        if (await _postService.PostExist(authorId, id) == false)
            return false;

        await _postService.DeletePost(authorId, id);
        return true;
    }

    public async Task<bool> PostIsLikedByUser(Guid userId, Guid id)
    {
        return await _postService.PostIsLikedByUser(userId, id);
    }

    public async Task LikePost(Guid userId, Guid id)
    {
        await _postService.LikePost(id, userId);
    }

    public async Task RemoveLikeFromPost(Guid authorId, Guid id)
    {
        await _postService.RemoveLikeFromPost(id, authorId);
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
    
    private int GetSkipCount(int page)
    {
        return  page * _postsPerPage;
    }
    
    private async Task<bool> PageIsValid(int page, Guid? userId = null)
    {
        var postsCount = _postService.GetPostsCount();

        return page >= 0 && page <= await GetPagesCount(userId);
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
}
