using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foodiefeed.models.dto
{
    public class CommentDto
    {
        public int CommentId { get; set; }
        public string CommentContent { get; set; }
        public int Likes { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; }
    }
}
