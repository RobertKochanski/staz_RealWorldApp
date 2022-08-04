using AutoMapper;
using RealWorldApp.Commons.Entities;
using RealWorldApp.Commons.Models.ArticleModel;
using RealWorldApp.Commons.Models.CommentModel;
using RealWorldApp.Commons.Models.UserModel;
using RealWorldApp.CQRS.Users.Commends;

namespace RealWebAppAPI
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, ViewUserModel>();
            CreateMap<User, UserResponse>();
            CreateMap<User, ProfileResponse>();
            CreateMap<CreateUserCommand, User>().ReverseMap();

            CreateMap<Article, ArticleResponseModel>()
                .ForMember(x => x.CreatedAt, opt => opt.MapFrom(x => x.CreateDate))
                .ForMember(x => x.UpdateAt, opt => opt.MapFrom(x => x.UpdateDate))
                .ForMember(x => x.Author, opt => opt.MapFrom(x => x.Author))
                .ForMember(x => x.TagList, opt => opt.MapFrom(x => x.TagList.Select(x => x.Name)));

            CreateMap<User, UserArticleResponseModel>();
            
            CreateMap<Comment, CommentResponseModel>();

        }
    }
}
