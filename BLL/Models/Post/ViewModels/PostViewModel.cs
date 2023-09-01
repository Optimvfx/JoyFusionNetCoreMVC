namespace BLL.Models.Post.ViewModels;

public class PostViewModel
{
    public Guid Id { get; set; }
    
    public string Title { get; set; }
    
    public string Content { get; set; }
    
    public DateTime PublishDate { get; set; }

    public Guid AuthorId { get; set; }

    public int LikesCount { get; set; }

    public int CommentsCount  { get; set; }
    
    public ICollection<byte[]> ImagesData { get; set; } = null!;
}