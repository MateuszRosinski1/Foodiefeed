using Foodiefeed_api.entities;
using Microsoft.EntityFrameworkCore;
using Foodiefeed_api.exceptions;
using AutoMapper;
using Foodiefeed_api.models.notifications;

namespace Foodiefeed_api.services
{
    public interface INotificationService
    {
        Task CreateNotification(NotificationType type, int senderId, int ReceiverId, string nickname);
        Task CreateNotification(NotificationType type, int senderId, int ReceiverId, string nickname, int Id, NotificationTarget target);
        Task CreateNotification(NotificationType type, int senderId, int ReceiverId, string nickname, int postId, int commentId);
        Task RemoveRange(List<int> ids);

        Task<List<NotificationDto>> GetNotificationByUserId(int id);
    }

    public class NotificationService : INotificationService
    {
        private readonly dbContext _dbContext;
        private readonly IAzureBlobStorageSerivce AzureBlobStorageSerivce;
        private readonly IMapper _mapper;

        public NotificationService(dbContext dbContext,IMapper mapper, IAzureBlobStorageSerivce azureBlobStorageSerivce)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            AzureBlobStorageSerivce = azureBlobStorageSerivce;
        }

        public async Task RemoveRange(List<int> ids)
        {
            var notifications = _dbContext.Notifications.Where(n => ids.Contains(n.Id)).ToList();

            _dbContext.Notifications.RemoveRange(notifications);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<NotificationDto>> GetNotificationByUserId(int id)
        {
            var notifications = await _dbContext.Notifications.Where(n => n.ReceiverId == id).ToListAsync();

            if (notifications is null) { throw new NotFoundException("No Notifications found."); }

            var notificationsDtos = _mapper.Map<List<NotificationDto>>(notifications);

            foreach (var dto in notificationsDtos)
            {
                var imgStream = await AzureBlobStorageSerivce.FetchProfileImageAsync(dto.SenderId);
                dto.Base64 = await AzureBlobStorageSerivce.ConvertStreamToBase64Async(imgStream);
            }

            return notificationsDtos;
        }

        public  async Task CreateNotification(NotificationType type, int senderId, int ReceiverId,string nickname) //Friend request , accepted friend request, gain follower
        {
            var notification = new Notification(type,nickname) { SenderId = senderId ,ReceiverId = ReceiverId};

            _dbContext.Notifications.Add(notification);

            await _dbContext.SaveChangesAsync();         
        }

        public async Task CreateNotification(NotificationType type, int senderId, int ReceiverId, string nickname,int Id) //postlike  commentLike
        {
            Notification notification;
            if (type == NotificationType.PostLike)
            {
                notification = new Notification(type, nickname) { SenderId = senderId, ReceiverId = ReceiverId, PostId = Id };
            }
            else
            {
                notification = new Notification(type, nickname) { SenderId = senderId, ReceiverId = ReceiverId, CommentId = Id };

            }
            _dbContext.Notifications.Add(notification);

            await _dbContext.SaveChangesAsync();
        }

        public async Task CreateNotification(NotificationType type, int senderId, int ReceiverId, string nickname, int postId,int commentId)// post comment
        {
            var notification = new Notification(type, nickname) { SenderId = senderId, ReceiverId = ReceiverId,PostId = postId,CommentId = commentId };

            _dbContext.Notifications.Add(notification);

            await _dbContext.SaveChangesAsync();
        }
    }

}
