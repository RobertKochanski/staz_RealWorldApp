using AutoMapper;
using RealWorldApp.Commons.Entities;
using RealWorldApp.Commons.Models.ArticleModel;
using RealWorldApp.Commons.Models.CommentModel;
using RealWorldApp.Commons.Models.UserModel;

namespace RealWebAppAPI
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, ViewUserModel>();
            CreateMap<User, UserResponse>();
            CreateMap<User, ProfileResponse>();

            CreateMap<Article, ArticleResponseModel>()
                .ForMember(x => x.CreatedAt, opt => opt.MapFrom(x => x.CreateDate))
                .ForMember(x => x.UpdateAt, opt => opt.MapFrom(x => x.UpdateDate))
                .ForMember(x => x.Author, opt => opt.MapFrom(x => x.Author));

            CreateMap<User, UserArticleResponseModel>();
            
            CreateMap<Comment, CommentResponseModel>();

        }
    }
}
