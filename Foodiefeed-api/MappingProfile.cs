using AutoMapper;
using Foodiefeed_api.entities;
using Foodiefeed_api.models.comment;
using Foodiefeed_api.models.friends;
using Foodiefeed_api.models.notifications;
using Foodiefeed_api.models.posts;
using Foodiefeed_api.models.user;

namespace Foodiefeed_api
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateUserDto, User>();
            CreateMap<User, ListedFriendDto>();
            CreateMap<User,UserDto>();
            CreateMap<User, UserProfileModel>();
            CreateMap<Post, PostDto>();
            CreateMap<Comment,CommentDto>();
            CreateMap<Notification, NotificationDto>();
        }
    }
}
