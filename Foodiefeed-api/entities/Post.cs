using System.ComponentModel.DataAnnotations.Schema;

namespace Foodiefeed_api.entities
{
    public class Post
    {
        public int PostId { get; set; }
        public int UserId { get; set; }
        public string Description { get; set; }
        
        public DateTime CreateTime { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        public virtual ICollection<PostCommentMember> PostCommentMembers { get; set; }
        public virtual ICollection<PostProduct> PostProducts { get; set; }
        public virtual ICollection<PostTag> PostTags { get; set; }
        public virtual ICollection<PostLike> PostLikes { get; set; }

    }
}
