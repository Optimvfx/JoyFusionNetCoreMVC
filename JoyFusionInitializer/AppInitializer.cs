using BLL.Models.Auth;
using BLL.Models.Post;
using BLL.Services;
using CCL.ControllersLogic;
using Common.Extensions;
using Common.Models;
using DAL;
using JoyFusionInitializer.Models;

namespace JoyFusionInitializer;

public class AppInitializer
{
    private readonly ServiceProvider _provider;
    
    private readonly ILogger<AppInitializer> _logger;

    public AppInitializer(ServiceProvider serviceProvider)
    {
        _provider = serviceProvider;
        _logger = _provider.GetService<ILogger<AppInitializer>>() ?? throw new NullReferenceException();
    }

    public async Task Initialize(InitializeConfigModel configModel, Procent reactionProcent, Procent subscribeProcent)
    {
        InitializeRoles(configModel);
        
        var userIds = InitializeUsers(configModel.UserModels);
        
        var postCreateRequests = GenerateRequests(userIds, configModel);
        var postsIds = await InitializePostsMultithread(postCreateRequests);
        
        await InitializeCommentsLikesMultithread(userIds, postsIds, reactionProcent);
        await InitializeSubscribesMultithread(userIds, subscribeProcent);
    }

    private void InitializeRoles(InitializeConfigModel configModel)
    {
        var db = _provider.GetService<ApplicationDbContext>() ?? throw new NullReferenceException();
        
        if (!db.IsInitialized())
        {
            ApplicationDbInitializer.InitializeRoles(db);
        }
    }

    private Guid[] InitializeUsers(RegisterModel[] registerModels)
    {
        var userService = _provider.GetService<UserService>() ?? throw new NullReferenceException();

        var userIds = new Guid[registerModels.Length];
        
        for(int i = 0; i < registerModels.Length; i++)
        {
            _logger.LogProgress(i , registerModels.Length, "InitializeUsers", false);

            var model = registerModels[i];
            
            var userId = userService.RegisterUser(model).Result;
            userIds[i] = userId;
        }

        return userIds;
    }

    private async Task<List<Guid>> InitializePostsMultithread(IEnumerable<PostCreateRequest> postCreateRequests)
    {
        var tasks = new List<Task<Guid>>();

        foreach (var postCreateRequest in postCreateRequests)
        {
            foreach (var post in postCreateRequest.PostsToCreate)
            {
                Task<Guid> task = Task.Run(async () => await InitializePost(postCreateRequest.AuthorId, post));

                tasks.Add(task);
            }
        }

        var totalCount = tasks.Count();
        ReferenseType<int> complitedCount = new(0);
        
        foreach (var task in tasks)
        {
            task.ContinueWith(t =>
            {
                lock (complitedCount)
                {
                    complitedCount.Value++;
                }

                lock (_logger)
                {
                    _logger.LogProgress(complitedCount.Value, totalCount, "InitializePosts", false);
                }
            });
        }
        
        await Task.WhenAll(tasks);

        return tasks.Select(t => t.Result).ToList();
    }

    private async Task<Guid> InitializePost(Guid userId, PostCreateModel post)
    {
        try
        {
            PostControllerLogic postLogic = null;
                    
            lock (_provider)
            {
                postLogic = _provider.GetService<PostControllerLogic>() ?? throw new NullReferenceException();
            }
                    
            var result = await postLogic.TryCreate(userId, post);

            if (result.IsFailure())
                throw new ArgumentException();

            return result.Value;
        }
        catch (Exception e)
        {
            lock (_logger)
            {
                _logger.LogError($"Post creation failed.\n"
                + e.ToString());
            }
                    
            throw e;
        }
    }
    
    private async Task InitializeCommentsLikesMultithread(IEnumerable<Guid> userIds, IEnumerable<Guid> postIds, Procent reactionProcent)
    {
        var rand = new Random();
        var tasks = new List<Task>();

        foreach (var postId in postIds)
        {
            Task task = Task.Run(async () => await InitializeCommentOrLike(userIds, postId, rand, reactionProcent));
            tasks.Add(task);
        }

        var totalCount = tasks.Count();
        ReferenseType<int> complitedCount = new(0);

        foreach (var task in tasks)
        {
            task.ContinueWith(t =>
            {
                lock (complitedCount)
                {
                    complitedCount.Value++;
                }

                lock (_logger)
                {
                    _logger.LogProgress(complitedCount.Value, totalCount, "Comments and Likes", false);
                }
            });
        }
        
        await Task.WhenAll(tasks);
    }

    private async Task InitializeCommentOrLike(IEnumerable<Guid> userIds, Guid postId, Random rand,
        Procent reactionProcent)
    {
        ReactionsControllerLogic reactionsLogic = null;

        lock (_provider)
        {
            reactionsLogic = _provider.GetService<ReactionsControllerLogic>() ?? throw new NullReferenceException();
        }

        foreach (var userId in userIds)
        {
            if (rand.NextBool(reactionProcent) == false)
                continue;

            try
            {
                if (rand.NextBool())
                {
                    var result = await reactionsLogic.TryAddComment(userId, postId, rand.NextString(20));
                }
                else
                {
                    await reactionsLogic.LikePost(userId, postId);
                }
            }
            catch (Exception e)
            {
                lock (_logger)
                {
                    _logger.LogError($"Comment creation failed.\n" + e.ToString());
                }
            }
        }
    }

    private async Task InitializeSubscribesMultithread(IEnumerable<Guid> userIds, Procent subscribeProcent)
    {
        var rand = new Random();
        var tasks = new List<Task>();

        foreach (var subscriberId in userIds)
        {
            foreach (var targetId in userIds)
            {
                if(subscriberId == targetId)
                    continue;

                if (rand.NextBool(subscribeProcent))
                {
                    var task = Task.Run(async () =>
                    {
                        await InitializeSubscription(subscriberId, targetId);
                    });

                    tasks.Add(task);
                }
            }
        }

        var totalCount = tasks.Count();
        ReferenseType<int> complitedCount = new(0);

        foreach (var task in tasks)
        {
            task.ContinueWith(t =>
            {
                lock (complitedCount)
                {
                    complitedCount.Value++;
                }

                lock (_logger)
                {
                    _logger.LogProgress(complitedCount.Value, totalCount, "Subscribes", false);
                }
            });
        }
        
        await Task.WhenAll(tasks);
    }

    private async Task InitializeSubscription(Guid userId, Guid targetId)
    {
        try
        {
            ReactionsControllerLogic reactionsLogic = null;
                    
            lock (_provider)
            {
                reactionsLogic = _provider.GetService<ReactionsControllerLogic>() ?? throw new NullReferenceException();
            }

            var result = await reactionsLogic.TrySubscribe(userId, targetId);

            if (result == false)
                throw new ArgumentException();
        }
        catch (Exception e)
        {
            lock (_logger)
            {
                _logger.LogError($"Comment creation failed.\n"
                                 + e.ToString());
            }
                    
            throw e;
        }
    }
    
    private IEnumerable<PostCreateRequest> GenerateRequests(Guid[] userIds, InitializeConfigModel configModel)
    {
        if (userIds.Length != configModel.UserModels.Length)
            throw new ArgumentException();

        for (int i = 0; i < userIds.Length; i++)
        {
            var newPostCreateRequest = new PostCreateRequest(
                configModel.UserModels[i].PostCreateModels,
                userIds[i]);

            yield return newPostCreateRequest;
        }
    }

    private class PostCreateRequest
    {
        public readonly IEnumerable<PostCreateModel> PostsToCreate;
        public readonly Guid AuthorId;

        public PostCreateRequest(IEnumerable<PostCreateModel> postsToCreate, Guid authorId)
        {
            PostsToCreate = postsToCreate;
            AuthorId = authorId;
        }
    }
}
