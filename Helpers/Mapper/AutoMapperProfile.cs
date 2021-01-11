using AutoMapper;
using dotnet.boilerplate.Dto;
using dotnet.boilerplate.Models;

namespace dotnet.boilerplate.Helpers.Mapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, ViewUserDto>();
            CreateMap<AddUserDto, User>();
            CreateMap<EditUserDto, User>();
            CreateMap<RegisterDto, User>();
        }
    }
}