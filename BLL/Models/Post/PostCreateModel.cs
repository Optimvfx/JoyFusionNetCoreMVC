namespace BLL.Models.Post;

public class PostCreateModel
{
    public string Title { get; set; }

    public string Content { get; set; }

    public ICollection<Image> Images { get; set; } = null!;
}