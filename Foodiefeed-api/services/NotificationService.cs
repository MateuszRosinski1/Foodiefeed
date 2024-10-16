using Foodiefeed_api.entities;
using Microsoft.EntityFrameworkCore;
using Foodiefeed_api.exceptions;
using AutoMapper;
using Foodiefeed_api.models.notifications;

namespace Foodiefeed_api.services
{
    public interface INotificationService
    {
        Task CreateNotification(NotificationType type, int sender, int Receiver);
        Task<List<NotificationDto>> GetNotificationByUserId(int id);
    }

    public class NotificationService : INotificationService
    {
        private readonly dbContext _dbContext;

        private readonly IMapper _mapper;

        public NotificationService(dbContext dbContext,IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<List<NotificationDto>> GetNotificationByUserId(int id)
        {
            var notifications = await _dbContext.Notifications.Where(n => n.ReceiverId == id).ToListAsync();

            if (notifications is null) { throw new NotFoundException("No Notifications found."); }

            var notificationsDtos = _mapper.Map<List<NotificationDto>>(notifications);

            return notificationsDtos;
        }

        public async Task CreateNotification(NotificationType type, int senderId, int ReceiverId)
        {
            var notification = new Notification(type) { SenderId = senderId ,ReceiverId = ReceiverId};

            _dbContext.Notifications.Add(notification);

            await _dbContext.SaveChangesAsync();         
        }
    }
}
