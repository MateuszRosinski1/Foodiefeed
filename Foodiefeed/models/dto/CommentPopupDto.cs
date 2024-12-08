using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foodiefeed.models.dto
{
    internal class CommentPopupDto
    {
        public string ProfilePictureImageBase64 { get; set; }
        public string Username { get; set; }
        public string CommentContent { get; set; }
        public string UserId { get; set; }

    }
}
