using AutoMapper;
using Foodiefeed_api.entities;
using Foodiefeed_api.exceptions;
using Foodiefeed_api.models.posts;
using Microsoft.EntityFrameworkCore;

namespace Foodiefeed_api.services
{
    public interface IPostService
    {
        public Task<List<PostDto>> GetProfilePostsAsync(string userId);
    }

    public class PostService : IPostService
    {
        private readonly dbContext _dbContext;
        private readonly IEntityRepository<User> _userRepository;
        private readonly IMapper _mapper;

        private struct Status
        {
            public const bool Online = true;
            public const bool Offline = false;
        }

        private async Task Commit() => await _dbContext.SaveChangesAsync();

        public PostService(dbContext dbContext, IEntityRepository<User> entityRepository, IMapper mapper)
        {
            _dbContext = dbContext;
            _userRepository = entityRepository;
            _mapper = mapper;
        }

        public async Task<List<PostDto>> GetProfilePostsAsync(string userId)
        {
            var user = _dbContext.Users.Include(u => u.Posts)
                .FirstOrDefault(u => u.Id == Convert.ToInt32(userId));

            if (user is null) { throw new NotFoundException("user not found"); }

            var posts = user.Posts.ToList();

            var postsDtos = _mapper.Map<List<PostDto>>(posts);

            int i = 0;
            foreach (var post in posts)
            {
                var postImages = post.PostImages.ToList();

                foreach(var image in postImages)
                {
                    using var imageEncodeStream = await FileSystem.OpenAppPackageFileAsync(image.ImagePath);
                    using var memoryStream = new MemoryStream();

                    imageEncodeStream.CopyTo(memoryStream);

                    var imageBase64 = Convert.ToBase64String(memoryStream.ToArray());

                    postsDtos[i].PostImagesBase64.Add(imageBase64);
                }

                var postProducts = post.PostProducts.ToList();

                foreach(var product in postProducts)
                {                    
                    postsDtos[i].ProductsName.Add(product.Product);
                }
                i++;

            }

            return postsDtos;
        }
    }
}
