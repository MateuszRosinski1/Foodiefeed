using AutoMapper;
using Foodiefeed_api.entities;
using Foodiefeed_api.models.user;
using Windows.System;

namespace Foodiefeed_api
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateUserDto, Foodiefeed_api.entities.User>();
        }
    }
}
