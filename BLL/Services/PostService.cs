using AutoMapper;
using BLL.Models.Post.Request;
using BLL.Services.TimeService;
using Common.Exceptions.Post;
using Common.Exceptions.User;
using DAL;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services;

public class PostService
{
    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;
    private readonly ITimeService _timeService;
    private readonly UserService _userService;

    public PostService(ApplicationDbContext db, IMapper mapper, ITimeService timeService, UserService userService)
    {
        _db = db;
        _mapper = mapper;
        _timeService = timeService;
        _userService = userService;
    }

    public async Task LikePost(Guid id, Guid userId)
    {
        if (await _userService.UserExistById(userId) == false)
            throw new UserNotFoundException();
        
        if (await PostExist(id) == false)
            throw new PostNotFoundException();

        await _db.Likes.AddAsync(new Like()
        {
            AuthorId = userId,
            PostId = id
        });
        await _db.SaveChangesAsync();
    }

    public async Task RemoveLikeFromPost(Guid id, Guid userId)
    {
        if (await _userService.UserExistById(userId) == false)
            throw new UserNotFoundException();
        
        if (await PostExist(id) == false ||
            await PostIsLikedByUser(userId, id) == false)
            throw new PostNotFoundException();

        var like = await _db.Likes.FirstAsync(l => l.AuthorId == userId && l.PostId == id);
       _db.Likes.Remove(like);
       await _db.SaveChangesAsync();
    }

    public async Task<bool> PostIsLikedByUser(Guid userId, Guid id)
    {
        if (await _userService.UserExistById(userId) == false)
            throw new UserNotFoundException();
        
        return await _db.Likes.AnyAsync(l => l.AuthorId == userId && l.PostId == id);
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

        var post = await GetByIdWithImages(id);
        _db.Posts.Remove(post);
        await _db.SaveChangesAsync();
    }

    public  async Task<Guid> CreatePost(Guid userId, PostCreateRequest request)
    {
        if (await _userService.UserExistById(userId) == false)
            throw new UserNotFoundException();
        
        var post = _mapper.Map<Post>(request);

        post.AuthorId = userId;
        post.PublishDate = _timeService.GetCurrentDateTime();
        
        await _db.AddAsync(post);
        await _db.SaveChangesAsync();

        return post.Id;
    }

    public async Task<Post> GetByIdWithImages(Guid id)
    {
        var post = await _db.Posts.AsNoTracking().Include(p => p.Images).FirstOrDefaultAsync(p => p.Id == id);

        if (post == null)
            throw new PostNotFoundException();

        return post;
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
    
    public async Task AddComment(Guid userId, Guid id, string content)
    {
        if (await _userService.UserExistById(userId) == false)
            throw new UserNotFoundException();
        
        if (await PostExist(id) == false)
            throw new PostNotFoundException();
        
        var comment = new Comment()
        {
            Content = content,
            AuthorId = userId,
            PostId = id,
            CreationDate = _timeService.GetCurrentDateTime()
        };

        await _db.Comments.AddAsync(comment);
        await _db.SaveChangesAsync();
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