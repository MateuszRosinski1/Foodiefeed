using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Foodiefeed_api.entities
{
    public class CommentLike
    {
        [Key]
        public int CommentId { get; set; }
        [Key]
        public int UserId { get; set; }

        [ForeignKey("CommentId")]
        public virtual Comment Comment{ get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}
