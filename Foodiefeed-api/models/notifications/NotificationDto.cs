using Foodiefeed_api.entities;

namespace Foodiefeed_api.models.notifications
{
    public class NotificationDto
    {

        public int Id { get; set; }

        public int SenderId { get; set; }

        public string Message { get; set; }
        public int? CommentId { get; set; }
        public int? PostId { get; set; }

        public DateTime CreatedAt { get; set; }


        public NotificationType Type { get; set; }
    }
}
