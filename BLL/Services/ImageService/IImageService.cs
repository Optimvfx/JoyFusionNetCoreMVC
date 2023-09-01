using Common.Models;
using SixLabors.ImageSharp;

namespace BLL.Services.ImageService;

public interface IImageService
{
    Result<Image> TryGetImage(byte[] bytes);
    Result<byte[]> TryGetBytes(Image image);
}