using System.ComponentModel.DataAnnotations.Schema;

namespace Foodiefeed_api.entities
{
    public class Comment
    {
        public int CommentId { get; set; }
        public string CommentContent { get; set; }
        //public int Likes { get; set; }
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        public virtual ICollection<CommentLike> CommentLikes { get; set; }

    }
}
