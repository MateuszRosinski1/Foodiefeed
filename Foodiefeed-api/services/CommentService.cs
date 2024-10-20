using AutoMapper;
using Foodiefeed_api.models.comment;
using Foodiefeed_api.entities;
using Microsoft.EntityFrameworkCore;
using Foodiefeed_api.exceptions;

namespace Foodiefeed_api.services
{
    public interface ICommentService
    {
        Task<CommentDto> GetCommentById(int id);
    }

    public class CommentService : ICommentService
    {
        private readonly dbContext _dbContext;
        private readonly IMapper _mapper;

        public CommentService(dbContext context,IMapper mapper)
        {
            _dbContext = context;
            _mapper = mapper;
        }

        public async Task<CommentDto> GetCommentById(int id)
        {
            var comment = _dbContext.Comments.Include(c => c.CommentLikes).FirstOrDefault(c => c.CommentId == id);

            if (comment is null) throw new NotFoundException("comment not found");

            var user = _dbContext.Users.FirstOrDefault(u => u.Id == comment.UserId);

            if(user is null)  throw new NotFoundException("user does not exist in current context."); 

            var commentDto = _mapper.Map<CommentDto>(comment);
            commentDto.Likes = comment.CommentLikes.ToList().Count();
            commentDto.Username = user.Username;
            //commentDto.ImageBase64 = 
            return commentDto; 
        }
    }
}
