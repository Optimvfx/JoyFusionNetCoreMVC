using Common.Models;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace BLL.Services.ImageService;

public class StandartImageService : IImageService
{
    public Result<Image> TryGetImage(byte[] bytes)
    {
        try
        {
            Image image = Image.Load(bytes);
            return new Result<Image>(ResultStatusCode.Success, image);
        }
        catch (Exception ex)
        {
            return new Result<Image>(ResultStatusCode.Failure, null);
        }
    }

    public Result<byte[]> TryGetBytes(Image image)
    {
        try
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                image.Save(memoryStream, new JpegEncoder());
                return new Result<byte[]>(ResultStatusCode.Success, memoryStream.ToArray());
            }
        }
        catch (Exception ex)
        {
            return new Result<byte[]>(ResultStatusCode.Failure, null);
        }
    }
}