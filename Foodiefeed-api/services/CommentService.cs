using AutoMapper;
using Foodiefeed_api.models.comment;
using Foodiefeed_api.entities;
using Microsoft.EntityFrameworkCore;
using Foodiefeed_api.exceptions;

namespace Foodiefeed_api.services
{
    public interface ICommentService
    {
        Task<CommentDto> GetCommentById(int id, CancellationToken token);
        Task<CommentDto> AddNewComment(int postId,NewCommentDto dto);
        Task EditComment(int commentId, string newContent);
        Task DeleteComment(int commentId);
        Task<int> LikeComment(int userId,int commentId);
        Task<int> UnlikeComment(int userId, int commentId);
    }

    public class CommentService : ICommentService
    {
        private readonly dbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IAzureBlobStorageSerivce AzureBlobStorageService;
        private readonly INotificationService _notificationService;
        private readonly IEntityRepository<User> _entityRepository;

        public CommentService(dbContext context,IMapper mapper, IAzureBlobStorageSerivce azureBlobStorageService, INotificationService notificationService,IEntityRepository<User> entityRepository)
        {
            _dbContext = context;
            _mapper = mapper;
            AzureBlobStorageService = azureBlobStorageService;
            _notificationService = notificationService;
            _entityRepository = entityRepository;
        }

        public async Task<CommentDto> AddNewComment(int postId, NewCommentDto dto)
        {
            var comment = _mapper.Map<Comment>(dto);

            var post = _dbContext.Posts.FirstOrDefault(p => p.PostId == postId);
            if (post is null) { throw new NotFoundException("Post you are trying to comment do not exist in current context."); }

            var user = await _entityRepository.FindByIdAsync(dto.UserId); // cannot be null
            var nickname = user.Username;

            _dbContext.Comments.Add(comment);
            await _dbContext.SaveChangesAsync();

            _dbContext.PostCommentMembers.Add(new PostCommentMember() { CommentId = comment.CommentId,PostId = postId });
            await _dbContext.SaveChangesAsync();

            await _notificationService.CreateNotification(NotificationType.PostComment, comment.UserId, post.UserId, nickname, post.PostId, comment.CommentId);
            return _mapper.Map<CommentDto>(comment);
        }

        public async Task EditComment(int commentId,string newContent)
        {
            var comment = await _dbContext.Comments.FirstOrDefaultAsync(c => c.CommentId == commentId);

            if (comment is null) { throw new NotFoundException("comment you trying to edit do not exist in current context"); }

            comment.CommentContent = newContent;
            await _dbContext.SaveChangesAsync();
        }

        public async Task<CommentDto> GetCommentById(int id, CancellationToken token)
        {
            var comment = _dbContext.Comments.Include(c => c.CommentLikes).FirstOrDefault(c => c.CommentId == id);

            if (comment is null) throw new NotFoundException("comment not found");

            var user = _dbContext.Users.FirstOrDefault(u => u.Id == comment.UserId);

            if(user is null)  throw new NotFoundException("user does not exist in current context.");

            token.ThrowIfCancellationRequested();

            var commentDto = _mapper.Map<CommentDto>(comment);
            commentDto.Likes = comment.CommentLikes.ToList().Count();
            commentDto.Username = user.Username;
            var commentPfpStream = await AzureBlobStorageService.FetchProfileImageAsync(comment.UserId,token);
            commentDto.ImageBase64 = await AzureBlobStorageService.ConvertStreamToBase64Async(commentPfpStream,token);
            return commentDto; 
        }

        public async Task DeleteComment(int commentId)
        {
            var comment = await _dbContext.Comments.FirstAsync(c => c.CommentId == commentId);
            var member = await _dbContext.PostCommentMembers.FirstAsync(m => m.CommentId == commentId);

            _dbContext.Comments.Remove(comment);
            _dbContext.PostCommentMembers.Remove(member);

            await _dbContext.SaveChangesAsync();
        }

        public async Task<int> LikeComment(int userId, int commentId)
        {
            var commentlike = await _dbContext.CommentLikes.FirstOrDefaultAsync(c => c.UserId == userId && c.CommentId == commentId);

            if (commentlike is not null) throw new BadRequestException($"Comment is already liked by the user with id:{userId}");

            _dbContext.CommentLikes.Add(new CommentLike { UserId = userId, CommentId = commentId });

            await _dbContext.SaveChangesAsync();

            var commentMember = await _dbContext.PostCommentMembers.FirstAsync(c => c.CommentId == commentId);

            return commentMember.PostId;
        }

        public async Task<int> UnlikeComment(int userId, int commentId)
        {
            var commentlike = await _dbContext.CommentLikes.FirstOrDefaultAsync(c => c.UserId == userId && c.CommentId == commentId);

            if (commentlike is null) throw new BadRequestException($"Comment is not liked by the user with id:{userId}");

            _dbContext.CommentLikes.Remove(commentlike);

            await _dbContext.SaveChangesAsync();

            var commentMember = await _dbContext.PostCommentMembers.FirstAsync(c => c.CommentId == commentId);

            return commentMember.PostId;
        }

    }
}
