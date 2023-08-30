using AutoMapper;
using BLL.Models.Post.Request;
using BLL.Models.Post.ViewModels;
using BLL.Services;
using CCL.ControllersLogic.Config;
using Common.Models;
using DAL.Entities;

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
        const int MinimalPostsPerPage = 1;

        var postsCount = _postService.GetPostsCount();

        if (postsCount < MinimalPostsPerPage)
            return new(ResultStatusCode.Success, new PostViewModel[0]);

        if (page < 0 || page > (postsCount - MinimalPostsPerPage) / _postsPerPage)
            return new(ResultStatusCode.Failure);

        var skipCount = _postsPerPage * page;

        return new(ResultStatusCode.Success, 
            _postService.GetPostTopByLike()
                .Skip(skipCount)
                .Take(_postsPerPage)
                .Select(p => _mapper.Map<PostViewModel>(p)));
    }

    public async Task<Result<Post>> TryGetById(Guid id)
    {
        if (await _postService.PostExist(id))
            return new(ResultStatusCode.Success,
                await _postService.GetByIdWithImages(id));

        return new(ResultStatusCode.Failure);
    }

    public async Task<Result<Guid>> TryCreate(Guid userId, PostCreateRequest request)
    {
        var newPostId = await _postService.CreatePost(userId, request);
        return new(ResultStatusCode.Success, newPostId);
    }

    public async Task<bool> TryDelete(Guid userId, Guid id)
    {
        if (await _postService.PostExist(userId, id) == false)
            return false;

        await _postService.DeletePost(userId, id);
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

    public async Task RemoveLikeFromPost(Guid userId, Guid id)
    {
        await _postService.RemoveLikeFromPost(id, userId);
    }
    
    public async Task<bool> PostExist(Guid userId, Guid id)
    {
        return await _postService.PostExist(userId, id);
    }
    
    public async Task<bool> PostExist(Guid id)
    {
        return await _postService.PostExist(id);
    }
}
