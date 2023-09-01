using System.Drawing;
using BLL.Models.Post;
using BLL.Services.ImageService;
using Common.Extensions;
using Common.Models;
using JoyFusionInitializer.Models;
using JoyFusionInitializer.Services;

namespace JoyFusionInitializer;

public class InitializeConfigModelGenerator
{
    public Random Random = new Random();
    
    private readonly RandomImageGeneratorService _randomImageGenerator;
    private readonly IImageService _imageService;

    private readonly ILogger<InitializeConfigModelGenerator> _logger;

    public InitializeConfigModelGenerator(RandomImageGeneratorService randomImageGenerator, IImageService imageService,
        ILogger<InitializeConfigModelGenerator> logger)
    {
        _randomImageGenerator = randomImageGenerator;
        _imageService = imageService;
        _logger = logger;
    }

    public InitializeConfigModel Generete(uint userCount, uint avgPostsPerUser, uint avgImagePerPost)
    {
        const int UserInfoLenght = 20;

        var userModels = new InitializeUserModel[userCount];

        for (int userIndex = 0; userIndex < userModels.Length; userIndex++)
        {
            _logger.LogProgress(userIndex, userModels.Length, "User Generation");

            var userPostsCount = GetValue(avgPostsPerUser);

            userModels[userIndex] = new InitializeUserModel()
            {
                Email = Random.NextString(UserInfoLenght),
                Nick = Random.NextString(UserInfoLenght),
                Password = Random.NextString(UserInfoLenght),
                PostCreateModels = GetPostCreateModels(userPostsCount, avgImagePerPost)
            };
        }

        return new InitializeConfigModel()
        {
            UserModels = userModels
        };
    }

    private PostCreateModel[] GetPostCreateModels(int count, uint avgImagePerPost)
    {
        const int PostTitleLenght = 50;
        const int PostContentLenght = 400;
        
        var postModels = new PostCreateModel[count];
        
        for (int postIndex = 0; postIndex < postModels.Length; postIndex++)
        {
            var postImagesCount = GetValue(avgImagePerPost);

            postModels[postIndex] = new PostCreateModel()
            {
                Title = Random.NextString(PostTitleLenght, Procent.Quarter),
                Content = Random.NextString(PostContentLenght, Procent.Quarter),
                Images = GetImages(avgImagePerPost)
            };
        }

        return postModels;
    }

    private List<Image> GetImages(uint count)
    {
        var imageModels = new List<Image>();

        for (int imageIndex = 0; imageIndex < count; imageIndex++)
        {
            try
            {
                var imageDataResult = _randomImageGenerator.GetImageDataAsync().Result;
                if (imageDataResult.IsFailure()) throw new ArgumentException();

                var imageResult = _imageService.TryGetImage(imageDataResult.Value);
                if (imageResult.IsFailure()) throw new AggregateException();

                imageModels.Add(imageResult.Value);
            }
            catch (Exception e)
            {
                _logger.LogError($"Image generation error: \n {e}");
            }
        }

        return imageModels;
    }

    private int GetValue(uint avgValue)
    {
        return Random.Next((int)avgValue * 2 + 1);
    }
}