using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Foodiefeed_api.entities
{
    public class Notification
    {
        public int Id { get; set; }

        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public int? CommentId { get; set; }
        public int? PostId { get; set; }

        public string Message { get; set; }

        [ForeignKey("SenderId")]
        public virtual User Sender { get; set; }
        [ForeignKey("ReceiverId")]
        public virtual User Receiver { get; set; }

        public NotificationType Type { get; set; }

        public Notification()
        {
            
        }

        public Notification(NotificationType type,string nickname)
        {
            Type = type;
            switch(type)
            {
                case NotificationType.FriendRequest:
                    this.Message = nickname + " has sended you a friend request.";
                    break;
                case NotificationType.PostLike:
                    this.Message = nickname + " has liked your post.";
                    break;
                case NotificationType.CommentLike:
                    this.Message = nickname + " has liked your comment.";
                    break;
                case NotificationType.PostComment:
                    this.Message = nickname + " commented your post.";
                    break;
                case NotificationType.GainFollower:
                    this.Message = nickname + " is now your follower.";
                    break;
                case NotificationType.AcceptedFriendRequest:
                    this.Message = "you are now friend with " + nickname;
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
