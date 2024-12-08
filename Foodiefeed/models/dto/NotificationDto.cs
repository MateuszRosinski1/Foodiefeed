using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foodiefeed.models.dto
{
    public class NotificationDto
    {
        public int Id { get; set; }

        public string Base64 { get; set; }

        public int SenderId { get; set; }

        public string Message { get; set; }

        public int? CommentId { get; set; }
        public int? PostId { get; set; }

        public NotificationType Type { get; set; }

        public DateTime CreatedAt { get; set; }

    }

    public enum NotificationType
    {
        FriendRequest,
        PostLike,
        PostComment,
        CommentLike,
        GainFollower,
        AcceptedFriendRequest
    }
}
