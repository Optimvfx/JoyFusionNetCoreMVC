namespace BLL.Models.Post.Request;

public class PostCreateRequest
{
    public string Title { get; set; }
    
    public string Content { get; set; }
    
    public ICollection<string> ImagesUrls { get; set; } = null!;
}