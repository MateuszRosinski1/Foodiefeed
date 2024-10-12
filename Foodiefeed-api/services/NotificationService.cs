using Foodiefeed_api.entities;

namespace Foodiefeed_api.services
{
    public interface INotificationService
    {
        Task CreateNotification(NotificationType type, int sender, int Receiver);
    }

    public class NotificationService : INotificationService
    {
        private readonly dbContext _dbContext;

        public NotificationService(dbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task CreateNotification(NotificationType type, int senderId, int ReceiverId)
        {
            var notification = new Notification(type) { SenderId = senderId ,ReceiverId = ReceiverId};

            _dbContext.Notifications.Add(notification);

            await _dbContext.SaveChangesAsync();
            
        }
    }
}
