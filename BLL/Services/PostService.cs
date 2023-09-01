using System.Drawing;
using AutoMapper;
using BLL.Models.Post;
using BLL.Services.ImageService;
using BLL.Services.TimeService;
using Common.Exceptions.Post;
using Common.Exceptions.User;
using Common.Models;
using DAL;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services;

public class PostService
{
    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;
    
    private readonly ITimeService _timeService;
    private readonly IImageService _imageService;
    
    private readonly UserService _userService;

    public PostService(ApplicationDbContext db, IMapper mapper, ITimeService timeService, IImageService imageService, UserService userService)
    {
        _db = db;
        _mapper = mapper;
        _timeService = timeService;
        _imageService = imageService;
        _userService = userService;
    }

    public async Task<bool> PostExist(Guid authorId, Guid id)
    {
        if (await _userService.UserExistById(authorId) == false)
            throw new UserNotFoundException();
        
        return await _db.Posts.AnyAsync(p => p.Id == id && p.AuthorId == authorId);
    }
    
    public async Task<bool> PostExist(Guid id)
    {
        return await _db.Posts.AnyAsync(p => p.Id == id);
    }

    public async Task DeletePost(Guid userId, Guid id)
    {
        if (await PostExist(userId, id) == false)
            throw new PostNotFoundException();

        var post = await _db.Posts.FirstAsync(p => p.Id == userId);
        _db.Posts.Remove(post);
        await _db.SaveChangesAsync();
    }

    public async Task<Guid> CreatePost(Guid userId, PostCreateModel model)
    {
        if (await _userService.UserExistById(userId) == false)
            throw new UserNotFoundException();

        var imageBytesResults = model.Images.Select(bitmap => _imageService.TryGetBytes(bitmap));

        if (imageBytesResults.Any(r => r.IsFailure()))
            throw new ApplicationException();
        
        var post = _mapper.Map<Post>(model);
        
        post.Images = imageBytesResults.Select(r => new ImageEntity()
        {
            ImageData = r.Value
        }).ToList();
        
        post.AuthorId = userId;
        post.PublishDate = _timeService.GetCurrentDateTime();
        
        await _db.Posts.AddAsync(post);
        await _db.SaveChangesAsync();

        return post.Id;
    }

    public IQueryable<Post> GetById(Guid id)
    {
        var posts = _db.Posts.AsNoTracking().Where(post => post.Id == id);

        if (posts.Any() == false)
            throw new PostNotFoundException();

        return posts;
    }

    public IEnumerable<Post> GetUserPosts(PostOrderType orderType, Guid userId, int skip, int take)
    {
        //Using PostOrderType to walid Skip Take methods.
        //Take Skip Dont work if IQueryable<Post> send to other method.
        
        if (skip < 0 && take <= 0)
            throw new ArgumentException();

        if (_userService.UserExistById(userId).Result == false)
            throw new UserNotFoundException();

        if(orderType == PostOrderType.ByLikes)
            return _db.Posts
                .Include(p => p.Images)
                .AsNoTracking()
                .Where(p => p.AuthorId == userId)
                .OrderByDescending(p => p.LikesCount)
                .ThenByDescending(p => p.CommentsCount)
                .Skip(skip)
                .Take(take);
        if(orderType == PostOrderType.ByDate)
            return _db.Posts
                .Include(p => p.Images)
                .AsNoTracking()
                .Where(p => p.AuthorId == userId)
                .OrderByDescending(p => p.PublishDate)
                .Skip(skip)  
                .Take(take);

        throw new ArgumentException();
    }
    
    public IQueryable<Post> GetPostTopByOrderType(PostOrderType orderType, int skip, int take)
    {
        //Using PostOrderType to walid Skip Take methods.
        //Take Skip Dont work if IQueryable<Post> send to other method.
        
        if (skip < 0 && take <= 0)
            throw new ArgumentException();
        
        if(orderType == PostOrderType.ByLikes)
            return _db.Posts.AsNoTracking()
                .Include(p => p.Images)
                .OrderByDescending(p => p.LikesCount)
                .ThenByDescending(p => p.CommentsCount)
                .Skip(skip)
                .Take(take);
        if(orderType == PostOrderType.ByDate)
            return _db.Posts.AsNoTracking()
                .Include(p => p.Images)
                .OrderByDescending(p => p.PublishDate)
                .Skip(skip)  
                .Take(take);

        throw new ArgumentException();
    }

    public async Task<int> GetPostsCount(Guid? userId = null)
    {
        if (userId == null)
            return _db.Posts.Count();

        if (await _userService.UserExistById(userId.Value) == false)
            throw new UserNotFoundException();

        return _userService.GetUsersById(userId.Value)
            .Include(u => u.Posts)
            .First().Posts.Count;
    }

    public enum PostOrderType
    {
        ByDate,
        ByLikes
    }
}