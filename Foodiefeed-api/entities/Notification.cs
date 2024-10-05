using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Foodiefeed_api.entities
{
    public class Notification
    {
        public int Id { get; set; }

        public int SenderId { get; set; }
        public int ReceiverId { get; set; }

        public string Message { get; set; }

        [ForeignKey("SenderId")]
        public virtual User Sender { get; set; }
        [ForeignKey("ReceiverId")]
        public virtual User Receiver { get; set; }

        public NotificationType Type { get; set; }

        public Notification(NotificationType type)
        {
            switch(type)
            {
                case NotificationType.FriendRequest:
                    this.Message = "has sended you a friend request.";
                    break;
                case NotificationType.PostLike:
                    this.Message = "has liked your post.";
                    break;
                case NotificationType.CommentLike:
                    this.Message = "has liked your comment.";
                    break;
                case NotificationType.PostComment:
                    this.Message = "commented your post.";
                    break;
                case NotificationType.GainFollower:
                    this.Message = "is now your follower.";
                    break;
                case NotificationType.AcceptedFriendRequest:
                    this.Message = "you are now friend with ";
                    break;

            }
        }

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
