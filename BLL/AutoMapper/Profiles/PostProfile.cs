using BLL.Models.Post.Request;
using BLL.Models.Post.ViewModels;
using DAL.Entities;

namespace BLL.AutoMapper.Profiles;

public class PostProfile : BaseAutoMapperProfile
{
    public PostProfile()
    {
        CreateMap<Post, PostViewModel>()
            .ForMember(p => p.CommentsCount, m => m.MapFrom(p => p.CommentsCount))
            .ForMember(p => p.LikesCount, m => m.MapFrom(p => p.LikesCount))
            .ForMember(p => p.ImagesUrl, m => m.MapFrom(p => p.Images.Select(i => i.Url)));
        CreateMap<PostCreateRequest, Post>()
            .ForMember(p => p.Images, m => m.MapFrom(p => p.ImagesUrls.Select(url => new Image()
            {
                Url = url
            })));
    }
}