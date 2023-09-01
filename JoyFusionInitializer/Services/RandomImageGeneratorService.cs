using Common.Extensions;
using Common.Models;

namespace JoyFusionInitializer.Services;

public class RandomImageGeneratorService
{
    private readonly IReadOnlyList<byte[]> _cashedImages;

    private readonly Random _random = new Random();

    private readonly ILogger<RandomImageGeneratorService> _logger;

    public RandomImageGeneratorService(Config config, ILogger<RandomImageGeneratorService> logger)
    {
        _logger = logger;
        
        _cashedImages = GenerateImages(new HttpClient(), config).ToList();
    }

    public async Task<Result<byte[]>> GetImageDataAsync()
    {
        int cashedImageIndex = _random.Next(_cashedImages.Count);
        return new(ResultStatusCode.Success, _cashedImages[cashedImageIndex]);
    }

    private IEnumerable<byte[]> GenerateImages(HttpClient httpClient, Config config)
    {
        var totalGenerations = config.Widthes.Count() * config.Heightes.Count() * config.UniqueGenerations;
        var currentGeneration = 0;
        
        foreach (var width in config.Widthes)
        {
            foreach (var height in config.Heightes)
            {
                for (int i = 0; i < config.UniqueGenerations; i++)
                {
                    var imageGenerationResult = GenerateImageAsync(httpClient, width, height).Result;
                    
                    if (imageGenerationResult.IsFailure()) throw new ArgumentException();

                    yield return imageGenerationResult.Value;

                    currentGeneration++;
                    _logger.LogProgress(currentGeneration, totalGenerations, "Image cash generation");
                }
            }
        }
    }
    
    private async Task<Result<byte[]>> GenerateImageAsync(HttpClient httpClient, int width, int height)
    {
        string url = $"https://picsum.photos/{width}/{height}";

        using (HttpResponseMessage response = await httpClient.GetAsync(url))
        {
            if (response.IsSuccessStatusCode)
            {
                return new(ResultStatusCode.Success,  
                    await response.Content.ReadAsByteArrayAsync());
            }
            else
            {
                return new(ResultStatusCode.Failure);
            }
        }
    }
    
    public class Config
    {
        public readonly IEnumerable<int> Widthes;
        public readonly  IEnumerable<int>  Heightes;
        public readonly uint UniqueGenerations;

        public Config(IEnumerable<int> widthes, IEnumerable<int> heightes, uint uniqueGenerations)
        {
            if (uniqueGenerations <= 0 || widthes.Count() <= 0 || heightes.Count() <= 0)
                throw new AggregateException();
            
            Widthes = widthes;
            Heightes = heightes;
            UniqueGenerations = uniqueGenerations;
        }
    }
}