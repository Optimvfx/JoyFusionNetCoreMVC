using BLL.Models.Post;
using BLL.Models.Post.ViewModels;
using DAL.Entities;

namespace BLL.AutoMapper.Profiles;

public class PostProfile : BaseAutoMapperProfile
{
    public PostProfile()
    {
        CreateMap<Post, PostViewModel>()
            .ForMember(p => p.ImagesData, m => m.MapFrom(p => p.Images.Select(i => i.ImageData)))
            .ForMember(p => p.CommentsCount, m => m.MapFrom(p => p.CommentsCount))
            .ForMember(p => p.LikesCount, m => m.MapFrom(p => p.LikesCount));

        CreateMap<Post, PostDetalizedViewModel>()
            .ForMember(p => p.ImagesData, m => m.MapFrom(p => p.Images.Select(i => i.ImageData)))
            .ForMember(p => p.CommentsCount, m => m.MapFrom(p => p.CommentsCount))
            .ForMember(p => p.LikesCount, m => m.MapFrom(p => p.LikesCount))
            .ForMember(p => p.Commens, m => m.Ignore());

        CreateMap<Comment, CommentViewModel>()
            .ForMember(c => c.AuthorNick, m => m.MapFrom(c => c.Author.Nick));
        
        CreateMap<PostCreateModel, Post>()
            .ForMember(p => p.Images, m => m.Ignore());
    }
}