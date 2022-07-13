using AutoMapper;
using RealWorldApp.BAL.Models;
using RealWorldApp.DAL.Entities;

namespace RealWebAppAPI
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, ViewUserModel>();
            CreateMap<User, UserResponse>();
        }
    }
}
