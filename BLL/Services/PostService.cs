using AutoMapper;
using BLL.Models.Post.Request;
using BLL.Services.TimeService;
using Common.Exceptions.Post;
using DAL;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services;

public class PostService
{
    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;
    private readonly ITimeService _timeService;

    public PostService(ApplicationDbContext db, IMapper mapper, ITimeService timeService)
    {
        _db = db;
        _mapper = mapper;
        _timeService = timeService;
    }

    public async Task LikePost(Guid id, Guid userId)
    {
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
        if (await PostExist(id) == false ||
            await PostIsLikedByUser(userId, id) == false)
            throw new PostNotFoundException();

        var like = await _db.Likes.FirstAsync(l => l.AuthorId == userId && l.PostId == id);
       _db.Likes.Remove(like);
       await _db.SaveChangesAsync();
    }

    public async Task<bool> PostIsLikedByUser(Guid userId, Guid id)
    {
        return await _db.Likes.AnyAsync(l => l.AuthorId == userId && l.PostId == id);
    }

    public async Task<bool> PostExist(Guid userId, Guid id)
    {
        return await _db.Posts.AnyAsync(p => p.Id == id && p.AuthorId == userId);
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

    public int GetPostsCount()
    {
        return _db.Posts.Count();
    }

    public IQueryable<Post> GetPostTopByLike()
    {
        return _db.Posts.AsNoTracking().OrderBy(p => p.LikesCount);
    }
}