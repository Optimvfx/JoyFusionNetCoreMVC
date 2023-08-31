using BLL.Models.Post.ViewModels;

namespace JoyFusion.Models;

public class PostsViewModel
{
    public readonly IEnumerable<PostViewModel> Posts;
    public readonly int CurrentPage;
    public readonly int PagesCount;

    public PostsViewModel(IEnumerable<PostViewModel> posts, int currentPage, int pagesCount)
    {
        Posts = posts;
        CurrentPage = currentPage;
        PagesCount = pagesCount;
    }
}