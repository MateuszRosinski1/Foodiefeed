using System.ComponentModel.DataAnnotations.Schema;

namespace Foodiefeed_api.models.comment
{
    public class CommentDto
    {
        public string CommentContent { get; set; }
        public int Likes { get; set; }
        public int UserId { get; set; }

    }
}
