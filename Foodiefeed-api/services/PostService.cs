using AutoMapper;
using Foodiefeed_api.entities;
using Foodiefeed_api.exceptions;
using Foodiefeed_api.models.posts;
using Microsoft.EntityFrameworkCore;
using Foodiefeed_api.models.comment;

namespace Foodiefeed_api.services
{
    public interface IPostService
    {
        public Task<List<PostDto>> GetProfilePostsAsync(string userId);
        public Task<PopupPostDto> GetPopupPostAsync(int id, int commentId);
        public Task<PopupPostDto> GetLikedPostAsync(int id);
        public Task CreatePostAsync(CreatePostDto dto);
        public Task DeletePostAsync(int postId,int userId);
    }

    public class PostService : IPostService
    {
        private readonly dbContext _dbContext;
        private readonly IEntityRepository<User> _userRepository;
        private readonly IMapper _mapper;
        private readonly IAzureBlobStorageSerivce AzureBlobStorageService;

        private async Task Commit() => await _dbContext.SaveChangesAsync();

        public PostService(dbContext dbContext, IEntityRepository<User> entityRepository, IMapper mapper, IAzureBlobStorageSerivce azureBlobStorageService)
        {
            _dbContext = dbContext;
            _userRepository = entityRepository;
            _mapper = mapper;
            AzureBlobStorageService = azureBlobStorageService;
        }

        public async Task CreatePostAsync(CreatePostDto dto)
        {
            var post = _mapper.Map<Post>(dto);

            post.CreateTime = DateTime.Now.ToUniversalTime();
            
            _dbContext.Posts.Add(post);
            await Commit();

            await AzureBlobStorageService.UploadPostImagesAsync(post.UserId,post.PostId,dto.Images);
        }

        public async Task<PopupPostDto> GetLikedPostAsync(int id)
        {
            var post = await _dbContext.Posts
                 .Include(p => p.PostLikes)
                 .Include(p => p.PostProducts)
                 .Include(p => p.PostLikes)
                 .FirstOrDefaultAsync(p => p.PostId == id);

            if (post is null) throw new NotFoundException("post do not exist in current context");

            var postUser = await _dbContext.Users.FirstOrDefaultAsync(up => up.Id == post.UserId);

            if (post is null) throw new NotFoundException("post do not exist in current context.");
            if (postUser is null) throw new NotFoundException("user do not exist in current context.");

            var popupPostDto = _mapper.Map<PopupPostDto>(post);
            popupPostDto.Username = postUser.Username;
            popupPostDto.Likes = post.PostLikes.ToList().Count();

            var imgStream = await AzureBlobStorageService.FetchProfileImageAsync(popupPostDto.UserId);
            popupPostDto.PosterProfilePictureBase64 = await AzureBlobStorageService.ConvertStreamToBase64Async(imgStream);


            return popupPostDto;
        }

        public async Task<PopupPostDto> GetPopupPostAsync(int id,int commentId)
        {
            var post = await _dbContext.Posts
                .Include(p => p.PostLikes)
                .Include(p => p.PostProducts)
                .Include(p => p.PostLikes)
                .FirstOrDefaultAsync(p => p.PostId == id);

            if (post is null) throw new NotFoundException("post do not exist in current context");

            var comment  = await _dbContext.Comments.Include(c => c.CommentLikes).FirstOrDefaultAsync(c => c.CommentId == commentId);

            if(comment is null) throw new NotFoundException("comment do not exist in current context."); 

            var postUser = await _dbContext.Users.FirstOrDefaultAsync(up => up.Id == post.UserId);

            var commentUser = await _dbContext.Users.FirstOrDefaultAsync(uc => uc.Id == comment.UserId);

            var popupPostDto = _mapper.Map<PopupPostDto>(post);
            popupPostDto.Username = postUser.Username;
            popupPostDto.Likes = post.PostLikes.ToList().Count();
            popupPostDto.CommentLikes = comment.CommentLikes.ToList().Count().ToString();
            popupPostDto.CommentUsername = commentUser.Username;
            popupPostDto.CommentContent = comment.CommentContent;
            popupPostDto.CommentUserId = commentUser.Id.ToString();

            var pfpImgStream = await AzureBlobStorageService.FetchProfileImageAsync(popupPostDto.UserId);
            var commentImgStream = await AzureBlobStorageService.FetchProfileImageAsync(Convert.ToInt32(popupPostDto.CommentUserId));
            popupPostDto.PosterProfilePictureBase64 = await AzureBlobStorageService.ConvertStreamToBase64Async(pfpImgStream);
            popupPostDto.CommentProfilePictureImageBase64 = await AzureBlobStorageService.ConvertStreamToBase64Async(commentImgStream);

            return popupPostDto;
        }

        public async Task<List<PostDto>> GetProfilePostsAsync(string userId)
        {
            var user = await _dbContext.Users
            .Include(u => u.Posts)
                .ThenInclude(p => p.PostImages)
            .Include(u => u.Posts)
                .ThenInclude(p => p.PostProducts)
            .Include(u => u.Posts)
                .ThenInclude(p => p.PostCommentMembers)
            .Include(u => u.Posts)
                .ThenInclude(u => u.PostLikes)
            .FirstOrDefaultAsync(u => u.Id == Convert.ToInt32(userId));

            if (user is null) { throw new NotFoundException("user not found"); }

            var posts = user.Posts.ToList();
            var postsDtos = _mapper.Map<List<PostDto>>(posts);
                  
            int i = 0;
            foreach (var post in posts)
            {
                postsDtos[i].ConvertDateTimeToTimeSpan(post.CreateTime);
                postsDtos[i].Username = user.Username;
                postsDtos[i].Likes = post.PostLikes.Count();

                var pfpStream = await AzureBlobStorageService.FetchProfileImageAsync(postsDtos[i].UserId);
                postsDtos[i].ProfilePictureBase64 = await AzureBlobStorageService.ConvertStreamToBase64Async(pfpStream);

                var postCommentMembers = post.PostCommentMembers.ToList();
                postsDtos[i].Comments = new List<CommentDto>();

                foreach (var postComment in postCommentMembers)
                {
                    var comment = await _dbContext.Comments
                         .Include(u => u.CommentLikes)
                        .FirstOrDefaultAsync(c => c.CommentId == postComment.CommentId);

                    if(comment is null) { break; }

                    var commentDto = _mapper.Map<CommentDto>(comment);

                    var username = _userRepository.FindById(comment.UserId).Username;

                    if (username is null) { break; }
                    commentDto.Username = username;
                    commentDto.Likes = comment.CommentLikes.Count();

                    var commentPfpStream = await AzureBlobStorageService.FetchProfileImageAsync(comment.UserId);
                    commentDto.ImageBase64 = await AzureBlobStorageService.ConvertStreamToBase64Async(commentPfpStream);

                    postsDtos[i].Comments.Add(commentDto);
                }

                var imgStream = await AzureBlobStorageService.FetchPostImagesAsync(post.UserId, post.PostId);
                postsDtos[i].PostImagesBase64 = await AzureBlobStorageService.ConvertStreamToBase64Async(imgStream);

                

                var postProducts = post.PostProducts.ToList();
                postsDtos[i].ProductsName = new List<string>();

                foreach (var product in postProducts)
                {                    
                    postsDtos[i].ProductsName.Add(product.Product);
                }
                i++;

            }

            return postsDtos;
        }

        public async Task DeletePostAsync(int postId, int userId)
        {
             List<Comment> comments = new List<Comment>();

            var commentmembers = _dbContext.PostCommentMembers.Where(m => m.PostId == postId).ToList();

            foreach (var member in commentmembers)
            {
                var comment = await _dbContext.Comments.FirstAsync(c => c.CommentId == member.CommentId);
                comments.Add(comment);
            }

            var post = await _dbContext.Posts.FirstAsync(p => p.PostId == postId);

            _dbContext.PostCommentMembers.RemoveRange(commentmembers);
            _dbContext.Comments.RemoveRange(comments);
            _dbContext.Posts.Remove(post);
            await _dbContext.SaveChangesAsync();

            await AzureBlobStorageService.RemvePostImagesRangeAsync(userId, postId);      
        }
    }
}
