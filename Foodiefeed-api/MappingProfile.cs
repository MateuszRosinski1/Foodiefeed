using AutoMapper;
using Foodiefeed_api.entities;
using Foodiefeed_api.models.comment;
using Foodiefeed_api.models.post;
using Foodiefeed_api.models.PostTag;
using Foodiefeed_api.models.user;
using Windows.System;

namespace Foodiefeed_api
{
    public class MappingProfile : Profile
    {
        public MappingProfile(IMapper mapper)
        {
            CreateMap<CreateUserDto, entities.User>();         

            CreateMap<Comment, CommentDto>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.Likes, opt => opt.MapFrom(src => src.Likes))
                .ForMember(dest => dest.CommentContent, opt => opt.MapFrom(src => src.CommentContent));

            CreateMap<PostTag,PostTagDto>()
                .ForMember(dest => dest.PostId, opt => opt.MapFrom(src => src.PostId))
                .ForMember(dest => dest.TagName, opt => opt.MapFrom(src => src.TagName))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));


            CreateMap<Post, PostDto>()
                .ForMember(dest => dest.imagePath, opt => opt.MapFrom(src => src.PostImages.Select(img => img.ImagePath)))
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.PostTags.Select(tag => mapper.Map<PostTagDto>(tag))))
                .ForMember(dest => dest.Comments, opt => opt.MapFrom(src => src.PostCommentMembers.Select(commentMember => mapper.Map<CommentDto>(commentMember.Comment))));



        }
    }
}
