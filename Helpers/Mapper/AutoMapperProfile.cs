using AutoMapper;
using dotnet.boilerplate.Dto;
using dotnet.boilerplate.Models;

namespace dotnet.boilerplate.Helpers.Mapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<RegisterDto, User>();
            CreateMap<LoginDto, User>();

            CreateMap<User, ViewUserDto>();
            CreateMap<AddUserDto, User>();
            CreateMap<EditUserDto, User>();
        }
    }
}