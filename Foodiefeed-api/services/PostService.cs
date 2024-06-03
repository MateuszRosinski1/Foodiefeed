using AutoMapper;
using Foodiefeed_api.entities;
using Foodiefeed_api.models.post;
using Microsoft.VisualBasic;

namespace Foodiefeed_api.services
{
    public interface IPostService
    {      
        public IEnumerable<PostDto> getAllPostForUser(int userId);
    }

    public class PostService : IPostService
    {
        private readonly dbContext _dbContext;
        private readonly IMapper _mapper;
        public PostService(dbContext dbContext,IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        
        public IEnumerable<PostDto> getAllPostForUser(int userId)
        {
            var posts = _dbContext.Posts.Where(p => p.UserId == userId).ToList();

            if (!posts.Any()) {
                return null; // if there is no post to dispaly retun null. No exception needed to be thrown
            }

            var postsDto = _mapper.Map<List<PostDto>>(posts); 

            return postsDto;
        }

        
    }
}
