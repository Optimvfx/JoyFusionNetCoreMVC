using BLL.Services;
using BLL.Services.TimeService;

namespace CCL.ControllersLogic;

public class ReactionsControllerLogic
{
    private readonly PostControllerLogic _postControllerLogic;
    
    private readonly ReactionsService _reactionsService;
    private readonly UserService _userService;

    private readonly ITimeService _timeService;

    public ReactionsControllerLogic(PostControllerLogic postControllerLogic, ReactionsService reactionsService, UserService userService, ITimeService timeService)
    {
        _postControllerLogic = postControllerLogic;
        _reactionsService = reactionsService;
        _userService = userService;
        _timeService = timeService;
    }

    public async Task<bool> PostIsLikedByUser(Guid userId, Guid id)
    {
        return await _reactionsService.PostIsLikedByUser(userId, id);
    }

    public async Task LikePost(Guid userId, Guid id)
    {
        await _reactionsService.LikePost(id, userId);
    }

    public async Task RemoveLikeFromPost(Guid authorId, Guid id)
    {
        await _reactionsService.RemoveLikeFromPost(id, authorId);
    }
    
    public async Task<bool> TryAddComment(Guid userId, Guid id, string content)
    {
        if (await _userService.UserExistById(userId) == false)
            return false;

        if (await _postControllerLogic.PostExist(id) == false)
            return false;
        
        await _reactionsService.AddComment(userId, id, content);
        return true;
    }

    public async Task<bool> TrySubscribe(Guid subscriberId, Guid targetId)
    {
        if (subscriberId == targetId)
            return false;
        
        if (await _userService.UserExistById(subscriberId) == false)
            return false;
        
        if (await _userService.UserExistById(targetId) == false)
            return false;

        if (await _reactionsService.Subscribed(subscriberId, targetId))
            return false;

        await _reactionsService.Subscribe(subscriberId, targetId);
        return true;
    }
}
