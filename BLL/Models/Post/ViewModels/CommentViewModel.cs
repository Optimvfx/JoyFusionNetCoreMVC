using DAL.Entities;

namespace BLL.Models.Post.ViewModels;

public class CommentViewModel
{
    public string? Content { get; set; } = null;
    
    public DateTime CreationDate { get; set; }
      
    public Guid AuthorId { get; set; }
    public string AuthorNick { get; set; }
}