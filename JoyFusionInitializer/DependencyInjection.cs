using System;
using System.Collections;
using BLL.Models.Post.Request;
using BLL.Services;
using Common.Extensions;
using DAL;
using JoyFusionInitializer.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace JoyFusionInitializer;

public static class DependencyInjection
{
    public static void Initialize(this ServiceProvider services, InitializeConfigModel configModel)
    {
        InitializeRoles(services, configModel);
        InitializeUsers(services, configModel);
    }

    private static void InitializeRoles(ServiceProvider services, InitializeConfigModel configModel)
    {
        var dbContext = services.GetService<ApplicationDbContext>() ?? throw new NullReferenceException();

        if (!dbContext.IsInitialized())
        {
            ApplicationDbInitializer.InitializeRoles(dbContext);
        }
    }

    private static void InitializeUsers(ServiceProvider services, InitializeConfigModel configModel)
    {
       var userService = services.GetService<UserService>() ?? throw new NullReferenceException();
       var postService = services.GetService<PostService>() ?? throw new NullReferenceException();

       var postsIds = new List<Guid>();
       var userIds = new List<Guid>();
       
       foreach (var model in configModel.UserModels)
       {
          var userId = userService.RegisterUser(model).Result;
          
          userIds.Add(userId);
          postsIds.AddRange(InitializePostsByUsers(userId, model.PostModels, postService));
       }
       
       InitializeCommentsAndLikes(userIds, postsIds, postService);
    }

    private static IEnumerable<Guid> InitializePostsByUsers(Guid userId, IEnumerable<PostCreateRequest> posts, PostService service)
    {
        foreach (var post in posts)
        {  
            var newPostId = service.CreatePost(userId, post).Result;
            yield return newPostId;
        }
    }

    private static void InitializeCommentsAndLikes(IEnumerable<Guid> userIds, IEnumerable<Guid> postIds, PostService service)
    {
        var rand = new Random();
        
        foreach (var postId in postIds)
        {
            foreach (var userId in userIds)
            {
                if (rand.NextBool())
                {
                    service.AddComment(userId, postId, rand.NextString(20)).Wait();
                }
                if (rand.NextBool())
                {
                    service.LikePost(postId, userId).Wait();
                }
            }
        }
    }
}
