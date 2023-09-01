using DAL.Entities;

namespace BLL.Models.Post.ViewModels;

public class PostDetalizedViewModel : PostViewModel
{
    public ICollection<CommentViewModel> Commens { get; set; }
}