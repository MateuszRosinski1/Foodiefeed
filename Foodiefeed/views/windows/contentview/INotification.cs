using Foodiefeed.models.dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foodiefeed.views.windows.contentview
{
    public interface INotification
    {
        public Task HideAnimation(int distance, uint duration);
        NotificationType Type { get; set; }
    }
}
