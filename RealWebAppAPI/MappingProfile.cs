using AutoMapper;
using RealWorldApp.Commons.Entities;
using RealWorldApp.Commons.Models;

namespace RealWebAppAPI
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, ViewUserModel>();
            CreateMap<User, UserResponse>();
            CreateMap<User, ProfileResponse>();
        }
    }
}
