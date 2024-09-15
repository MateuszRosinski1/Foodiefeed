using AutoMapper;
using Foodiefeed_api.entities;
using Foodiefeed_api.models.friends;
using Foodiefeed_api.models.user;

namespace Foodiefeed_api
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateUserDto, entities.User>();
            CreateMap<User, OnlineFriendDto>();
        }
    }
}
