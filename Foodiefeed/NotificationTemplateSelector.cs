using Foodiefeed.models.dto;
using Foodiefeed.views.windows.contentview;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foodiefeed
{
    public class NotificationTemplateSelector : DataTemplateSelector
    {
        public DataTemplate FriendRequestNotificationTemplate { get; set; }
        public DataTemplate CommentLikeNotificationTemplate { get; set; }
        public DataTemplate PostLikeNotificationTemplate { get; set; }
        public DataTemplate PostCommentNotificationTemplate { get; set; }
        public DataTemplate BasicNotificationTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var notification = item as INotification;

            if (notification is null)
             return null;

            switch (notification.Type)
            {
                case NotificationType.FriendRequest:
                    return FriendRequestNotificationTemplate;

                case NotificationType.CommentLike:
                    return CommentLikeNotificationTemplate;

                case NotificationType.PostLike:
                    return PostLikeNotificationTemplate;

                case NotificationType.PostComment:
                    return PostCommentNotificationTemplate;

                default:
                    return BasicNotificationTemplate; 
            }



        }
    }
}
