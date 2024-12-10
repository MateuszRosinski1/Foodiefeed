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
        public Task<List<PostDto>> GetProfilePostsAsync(int userId, int pageNumber, CancellationToken token);
        public Task<PopupPostDto> GetPopupPostAsync(int id, int commentId, CancellationToken token);
        public Task<PopupPostDto> GetLikedPostAsync(int id, CancellationToken token);
        public Task CreatePostAsync(CreatePostDto dto,CancellationToken token);
        public Task DeletePostAsync(int postId,int userId);
        public Task<List<PostDto>> GenerateWallPostsAsync(int userId,List<int> viewedPostsId, CancellationToken token, int pageSize);

        public Task LikePost(int userId,int postId);
        public Task UnlikePost(int userId, int postId);
    }

    public class PostService : IPostService
    {
        private readonly dbContext _dbContext;
        private readonly IEntityRepository<User> _userRepository;
        private readonly IMapper _mapper;
        private readonly IAzureBlobStorageSerivce AzureBlobStorageService;
        private readonly INotificationService notificationService;

        private async Task Commit() => await _dbContext.SaveChangesAsync();

        public PostService(dbContext dbContext, 
            IEntityRepository<User> entityRepository, 
            IMapper mapper, 
            IAzureBlobStorageSerivce azureBlobStorageService,
            INotificationService _notificationService)
        {
            _dbContext = dbContext;
            _userRepository = entityRepository;
            _mapper = mapper;
            AzureBlobStorageService = azureBlobStorageService;
            notificationService = _notificationService;
        }

        public async Task CreatePostAsync(CreatePostDto dto,CancellationToken token)
        {
            var post = _mapper.Map<Post>(dto);

            post.CreateTime = DateTime.Now.ToUniversalTime();

            var tags = _dbContext.Tags.Where(t => dto.TagsId.Contains(t.Id)).ToList();
            var products = _dbContext.Products.Where(p => dto.ProductsId.Contains(p.Id)).ToList();

            List<PostTag> pTags = new();
            List<PostProduct> pProducts = new();
            foreach (var tag in tags)
            {
                pTags.Add(new PostTag() { PostId = post.PostId, TagId = tag.Id });
            }

            foreach (var product in products)
            {
                pProducts.Add(new PostProduct() { ProductId = product.Id, PostId = product.Id });
            }
            post.PostTags = pTags;
            post.PostProducts = pProducts;

            _dbContext.Posts.Add(post);

            await Commit();


            if (dto.Images is not null)
            {
                await AzureBlobStorageService.UploadPostImagesAsync(post.UserId, post.PostId, dto.Images,token);
            }       
        }

        public async Task<PopupPostDto> GetLikedPostAsync(int id, CancellationToken token)
        {
            var post = await _dbContext.Posts
                 .Include(p => p.PostLikes)
                 .Include(p => p.PostProducts)
                    .ThenInclude(p => p.Product)
                 .Include(p => p.PostLikes)
                 .FirstOrDefaultAsync(p => p.PostId == id);

            token.ThrowIfCancellationRequested();

            if (post is null) throw new NotFoundException("post do not exist in current context");

            var postUser = await _dbContext.Users.FirstOrDefaultAsync(up => up.Id == post.UserId);

            if (post is null) throw new NotFoundException("post do not exist in current context.");
            if (postUser is null) throw new NotFoundException("user do not exist in current context.");

            var popupPostDto = _mapper.Map<PopupPostDto>(post);
            popupPostDto.Username = postUser.Username;

            token.ThrowIfCancellationRequested();

            popupPostDto.Likes = post.PostLikes.ToList().Count();

            var imgStream = await AzureBlobStorageService.FetchProfileImageAsync(popupPostDto.UserId,token);
            popupPostDto.PosterProfilePictureBase64 = await AzureBlobStorageService.ConvertStreamToBase64Async(imgStream, token);

            var imgStreams = await AzureBlobStorageService.FetchPostImagesAsync(popupPostDto.UserId, popupPostDto.PostId,token);

            popupPostDto.PostImagesBase64 = await AzureBlobStorageService.ConvertStreamToBase64Async(imgStreams, token);

            return popupPostDto;
        }

        public async Task<PopupPostDto> GetPopupPostAsync(int id,int commentId, CancellationToken token)
        {
            var post = await _dbContext.Posts
                .Include(p => p.PostLikes)
                .Include(p => p.PostProducts)
                    .ThenInclude(p => p.Product)
                .Include(p => p.PostLikes) // ?? check
                .FirstOrDefaultAsync(p => p.PostId == id);

            token.ThrowIfCancellationRequested();

            if (post is null) throw new NotFoundException("post do not exist in current context");

            var comment  = await _dbContext.Comments
                .Include(c => c.CommentLikes)
                .FirstOrDefaultAsync(c => c.CommentId == commentId);

            if(comment is null) throw new NotFoundException("comment do not exist in current context."); 

            var postUser = await _dbContext.Users.FirstOrDefaultAsync(up => up.Id == post.UserId);

            var commentUser = await _dbContext.Users.FirstOrDefaultAsync(uc => uc.Id == comment.UserId);

            token.ThrowIfCancellationRequested();

            var popupPostDto = _mapper.Map<PopupPostDto>(post);
            popupPostDto.Username = postUser.Username;
            popupPostDto.Likes = post.PostLikes.ToList().Count();
            popupPostDto.CommentLikes = comment.CommentLikes.ToList().Count().ToString();
            popupPostDto.CommentUsername = commentUser.Username;
            popupPostDto.CommentContent = comment.CommentContent;
            popupPostDto.CommentUserId = commentUser.Id.ToString();

            var pfpstream = await AzureBlobStorageService.FetchProfileImageAsync(popupPostDto.UserId, token);
            popupPostDto.PosterProfilePictureBase64 = await AzureBlobStorageService.ConvertStreamToBase64Async(pfpstream,token);

            var imgsStreams = await AzureBlobStorageService.FetchPostImagesAsync(popupPostDto.UserId, popupPostDto.PostId, token);
            popupPostDto.PostImagesBase64 = await AzureBlobStorageService.ConvertStreamToBase64Async(imgsStreams,token);

            var pfpImgStream = await AzureBlobStorageService.FetchProfileImageAsync(popupPostDto.UserId, token);
            var commentImgStream = await AzureBlobStorageService.FetchProfileImageAsync(Convert.ToInt32(popupPostDto.CommentUserId), token);

            popupPostDto.PosterProfilePictureBase64 = await AzureBlobStorageService.ConvertStreamToBase64Async(pfpImgStream,token);
            popupPostDto.CommentProfilePictureImageBase64 = await AzureBlobStorageService.ConvertStreamToBase64Async(commentImgStream, token);

            return popupPostDto;
        }

        public async Task<List<PostDto>> GetProfilePostsAsync(int userId,int pageNumber, CancellationToken token)
        {
            const int PAGE_SIZE = 3;

            var user = await _dbContext.Users
                .Include(u => u.Posts)
                    .ThenInclude(p => p.PostProducts)
                    .ThenInclude(pp => pp.Product)
                .Include(u => u.Posts)
                    .ThenInclude(p => p.PostCommentMembers)
                .Include(u => u.Posts)
                    .ThenInclude(u => u.PostLikes)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user is null) { throw new NotFoundException("user not found"); }

            token.ThrowIfCancellationRequested();

            var posts = user.Posts.ToList().Skip(pageNumber * PAGE_SIZE).Take(PAGE_SIZE);
            var postsDtos = _mapper.Map<List<PostDto>>(posts);
                  
            int i = 0;
            foreach (var post in posts)
            {
                token.ThrowIfCancellationRequested();

                postsDtos[i].TimeSpan = ConverterHelper.ConvertDateTimeToTimeSpan(post.CreateTime);
                postsDtos[i].Username = user.Username;
                postsDtos[i].Likes = post.PostLikes.Count();

                var pfpStream = await AzureBlobStorageService.FetchProfileImageAsync(postsDtos[i].UserId,token);
                postsDtos[i].ProfilePictureBase64 = await AzureBlobStorageService.ConvertStreamToBase64Async(pfpStream, token);

                var postCommentMembers = post.PostCommentMembers.ToList();
                postsDtos[i].Comments = new List<CommentDto>();

                foreach (var postComment in postCommentMembers)
                {
                    token.ThrowIfCancellationRequested();

                    var comment = await _dbContext.Comments
                         .Include(u => u.CommentLikes)
                        .FirstOrDefaultAsync(c => c.CommentId == postComment.CommentId);

                    if(comment is null) { break; }

                    var commentDto = _mapper.Map<CommentDto>(comment);

                    var username = _userRepository.FindById(comment.UserId).Username;

                    if (username is null) { break; }
                    commentDto.Username = username;
                    commentDto.Likes = comment.CommentLikes.Count();

                    var commentPfpStream = await AzureBlobStorageService.FetchProfileImageAsync(comment.UserId, token);
                    commentDto.ImageBase64 = await AzureBlobStorageService.ConvertStreamToBase64Async(commentPfpStream,token);

                    commentDto.IsLiked = await _dbContext.CommentLikes.FirstOrDefaultAsync(c => c.UserId == userId && c.CommentId == commentDto.CommentId) is null ? false : true;

                    postsDtos[i].Comments.Add(commentDto);
                }

                var like = await _dbContext.PostLikes
                    .FirstOrDefaultAsync(pl => pl.PostId == postsDtos[i].PostId && pl.UserId == userId);

                postsDtos[i].IsLiked = like is null ? false : true;

                var save = await _dbContext.Recipes
                    .FirstOrDefaultAsync(r => r.PostId == postsDtos[i].PostId && r.UserId == userId);

                postsDtos[i].IsSaved = save is null ? false : true;

                var imgStream = await AzureBlobStorageService.FetchPostImagesAsync(post.UserId, post.PostId, token);
                postsDtos[i].PostImagesBase64 = await AzureBlobStorageService.ConvertStreamToBase64Async(imgStream,token);

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

        public async Task<List<PostDto>> GenerateWallPostsAsync(int userId,List<int> viewedPostsId, CancellationToken token,int pageSize)
        {
                 var userTags = await _dbContext.UserTags
                    .Where(ut => ut.UserId == userId)
                    .ToDictionaryAsync(ut => ut.TagId, ut => ut.Score);

                 token.ThrowIfCancellationRequested();

                 var postsEnumerable = _dbContext.Posts
                 .Where(p => !viewedPostsId.Contains(p.PostId))
                 .Select(p => new
                 {
                    Post = p,
                    PostTags = p.PostTags.Select(pt => pt.TagId).ToList(),
                    SecondsSinceCreated = (DateTime.Now - p.CreateTime).TotalSeconds
                 })
                 .AsEnumerable();

                 var posts = postsEnumerable.Select(p => new
                 {
                    Post = p.Post,
                    TagScoreSum = p.PostTags
                        .Where(tagId => userTags.ContainsKey(tagId)) 
                        .Sum(tagId => userTags[tagId]),
                     DayScore = 100 / (p.SecondsSinceCreated / 86400 + 1)
                 })
                 .Select(p => new
                 {
                     Post = p.Post,
                     PostScore = p.TagScoreSum + (p.DayScore)
                 })
                .OrderByDescending(p => p.PostScore)
                .Take(15)
                .ToList();

                token.ThrowIfCancellationRequested();

                var orderedPostIds = posts.Select((p, index) => 
                                new { p.Post.PostId, Index = index }).ToList();

                var postEntities = await _dbContext.Posts
                    .Include(p => p.User)
                    .Include(p => p.PostLikes)
                    .Include(p => p.PostCommentMembers)
                        .ThenInclude(pm => pm.Comment)
                        .ThenInclude(c => c.CommentLikes)
                    .Include(p => p.PostProducts)
                        .ThenInclude(pp => pp.Product)
                    .Where(p => orderedPostIds.Select(op => op.PostId).Contains(p.PostId))
                    .ToListAsync();

                token.ThrowIfCancellationRequested();

                var sortedPosts = orderedPostIds
                    .Join(postEntities,
                          op => op.PostId,
                          pe => pe.PostId,
                          (op, pe) => new { op.Index, Post = pe })
                    .OrderBy(x => x.Index)  
                    .Select(x => x.Post)
                    .ToList();

                var dtos = _mapper.Map<List<PostDto>>(sortedPosts);

                await FullfillMissingPostFields(dtos,token,userId);
                
                return dtos;
        }

        public async Task LikePost(int userId, int postId)
        {
            var postLike = await _dbContext.PostLikes.FirstOrDefaultAsync(pl => pl.UserId == userId && pl.PostId == postId);

            if (postLike is not null) throw new BadRequestException("the post is currently liked.");

            PostLike pl = new PostLike() { PostId = postId,UserId = userId };

            _dbContext.PostLikes.Add(pl);
            await Commit();

            var post = await _dbContext.PostLikes
                .Include(p => p.User)
                .Include(p => p.Post)
                    .ThenInclude(p => p.PostTags)
                        .ThenInclude(pt => pt.Tag)
                .FirstAsync(pl => pl.PostId == postId);

            if (userId == post.UserId) return;

            await notificationService.CreateNotification(NotificationType.PostLike,userId,post.UserId,post.User.Username);


            var user = await _dbContext.Users
                .Include(u => u.UserTags)
                   .ThenInclude(ut => ut.Tag)
                .FirstAsync(u => u.Id == userId);

            var userTags = user.UserTags.OrderBy(ut => ut.Tag.Name).ToList(); 

            foreach (var postTag in post.Post.PostTags)
            {
                var userTagIndex = userTags.FindIndex(t => t.Tag.Name == postTag.Tag.Name);
                if (userTagIndex >= 0)
                {
                    userTags[userTagIndex].Score = Math.Min(100, userTags[userTagIndex].Score + 2); 

                    var oppositeIndex = userTags.Count - 1 - userTagIndex;
                    if (oppositeIndex >= 0 && oppositeIndex < userTags.Count && oppositeIndex != userTagIndex)
                    {
                        userTags[oppositeIndex].Score = Math.Max(1, userTags[oppositeIndex].Score - 2); 
                    }
                }
            }
            await Commit();
        }

        public async Task UnlikePost(int userId,int postId)
        {
            var postLike = await _dbContext.PostLikes.FirstOrDefaultAsync(pl => pl.UserId == userId && pl.PostId == postId);

            if (postLike is null) throw new BadRequestException("the post is not currently liked.");

            _dbContext.PostLikes.Remove(postLike);
            await Commit();

            var user = await _dbContext.Users
                            .Include(u => u.UserTags)
                                .ThenInclude(ut => ut.Tag)
                            .FirstAsync(u => u.Id == userId);

            var post = await _dbContext.Posts
                            .Include(p => p.PostTags)
                                .ThenInclude(pt => pt.Tag)
                            .FirstAsync(p => p.PostId == postId);

            var userTags = user.UserTags.OrderBy(ut => ut.Tag.Name).ToList();

            foreach (var postTag in post.PostTags)
            {
                var userTagIndex = userTags.FindIndex(t => t.Tag.Name == postTag.Tag.Name);
                if (userTagIndex >= 0)
                {
                    userTags[userTagIndex].Score = Math.Max(1, userTags[userTagIndex].Score - 2);

                    var oppositeIndex = userTags.Count - 1 - userTagIndex;
                    if (oppositeIndex >= 0 && oppositeIndex < userTags.Count && oppositeIndex != userTagIndex)
                    {
                        userTags[oppositeIndex].Score = Math.Min(100, userTags[oppositeIndex].Score + 2); 
                    }
                }
            }
            await Commit();
        }

        private async Task FullfillMissingPostFields(List<PostDto> dtos,CancellationToken token,int userId)
        {
            foreach (var dto in dtos)
            {
                var imgStreams = await AzureBlobStorageService
                                .FetchPostImagesAsync(dto.UserId, dto.PostId, token);
                dto.PostImagesBase64 = await AzureBlobStorageService
                                .ConvertStreamToBase64Async(imgStreams, token);
                var pfpStream = await AzureBlobStorageService
                                .FetchProfileImageAsync(dto.UserId, token);
                dto.ProfilePictureBase64 = await AzureBlobStorageService
                                .ConvertStreamToBase64Async(pfpStream, token);

                dto.Likes = _dbContext.Posts.First(p => p.PostId == dto.PostId).PostLikes.Count;

                dto.IsLiked = await _dbContext.PostLikes
                    .FirstOrDefaultAsync(pl => pl.PostId == dto.PostId && pl.UserId == userId)
                    is null ? false : true;

                dto.IsSaved = await _dbContext.Recipes
                    .FirstOrDefaultAsync(r => r.PostId == dto.PostId && r.UserId == userId)
                    is null ? false : true;

                token.ThrowIfCancellationRequested();

                foreach (var comment in dto.Comments)
                {
                    var entity = await _dbContext.Comments
                     .Include(u => u.CommentLikes)
                     .Include(e => e.User)
                    .FirstAsync(c => c.CommentId == comment.CommentId);
                    comment.Likes = entity.CommentLikes.ToList().Count;
                    comment.Username = entity.User.Username;

                    var stream = await AzureBlobStorageService
                                .FetchProfileImageAsync(comment.UserId, token);
                    comment.ImageBase64 = await AzureBlobStorageService
                                .ConvertStreamToBase64Async(stream, token);
                    var cl = await _dbContext.CommentLikes
                            .FirstOrDefaultAsync
                            (c => c.UserId == userId && c.CommentId == comment.CommentId);
                    comment.IsLiked = cl is null ? false : true;
                }
            }
        }
    }
}
