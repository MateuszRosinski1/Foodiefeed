using AutoMapper;
using Foodiefeed_api.entities;
using Foodiefeed_api.models.comment;
using Foodiefeed_api.models.friends;
using Foodiefeed_api.models.notifications;
using Foodiefeed_api.models.posts;
using Foodiefeed_api.models.user;
using Foodiefeed_api.models.recipe;

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


            CreateMap<Post, PostDto>()
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.Username))
                .ForMember(dest => dest.Comments, opt => opt.MapFrom(src => src.PostCommentMembers.Select(pm => pm.Comment)))
                .ForMember(dest => dest.ProductsName, opt => opt.MapFrom(src => src.PostProducts.Select(pp => pp.Product.Name).ToList()))
                .ForMember(dest => dest.TimeSpan, opt => opt.MapFrom(src => ConverterHelper.ConvertDateTimeToTimeSpan(src.CreateTime)));

            CreateMap<Post, PopupPostDto>()
                .ForMember(dest => dest.ProductsName, opt => opt.MapFrom(src => src.PostProducts.Select(pp => pp.Product.Name).ToList()))
                .ForMember(dest => dest.TimeSpan, opt => opt.MapFrom(src => ConverterHelper.ConvertDateTimeToTimeSpan(src.CreateTime)));

            CreateMap<CreatePostDto, Post>();
            CreateMap<Comment, CommentDto>()
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.Username))
                .ForMember(dest => dest.Likes, opt => opt.MapFrom(src => src.CommentLikes.Count));

            CreateMap<Notification, NotificationDto>();
            CreateMap<NewCommentDto, Comment>();

            CreateMap<Post, RecipeDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.PostId))
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Products, opt => opt.MapFrom(src => src.PostProducts.Select(pp => pp.Product.Name).ToList()))
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.Username));



        }
    }
}
