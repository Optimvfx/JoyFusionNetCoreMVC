using AutoMapper;
using BLL.Services.ImageService;
using BLL.Services.TimeService;
using Common.Exceptions.Post;
using Common.Exceptions.User;
using DAL;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services;

public class ReactionsService
{
    private readonly ApplicationDbContext _db;
    private readonly ITimeService _timeService;
    
    private readonly PostService _postService;
    private readonly UserService _userService;

    public ReactionsService(ApplicationDbContext db, ITimeService timeService, PostService postService, UserService userService)
    {
        _db = db;
        _timeService = timeService;
        _postService = postService;
        _userService = userService;
    }

    public async Task LikePost(Guid id, Guid userId)
    {
        if (await _userService.UserExistById(userId) == false)
            throw new UserNotFoundException();
        
        if (await _postService.PostExist(id) == false)
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
        
        if (await _postService.PostExist(id) == false ||
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
    
    public async Task AddComment(Guid userId, Guid id, string content)
    {
        if (await _userService.UserExistById(userId) == false)
            throw new UserNotFoundException();
        
        if (await _postService.PostExist(id) == false)
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
    
    public async Task<bool> Subscribed(Guid subscriberId, Guid targetId)
    {
        if (await _userService.UserExistById(subscriberId) == false)
            throw new UserNotFoundException();

        if (await _userService.UserExistById(targetId) == false)
            throw new UserNotFoundException();

        return await _db.Subscriptions.AnyAsync(s => s.SubscriberId == subscriberId && s.TargetUserId == targetId);
    }
    
    public async Task Subscribe(Guid subscriberId, Guid targetId)
    {
        if (await _userService.UserExistById(subscriberId) == false)
            throw new UserNotFoundException();

        if (await _userService.UserExistById(targetId) == false)
            throw new UserNotFoundException();

        var subscribe = new Subscription()
        {
            SubscriberId = subscriberId,
            TargetUserId = targetId
        };

        await _db.Subscriptions.AddAsync(subscribe);
        await _db.SaveChangesAsync();
    }
}