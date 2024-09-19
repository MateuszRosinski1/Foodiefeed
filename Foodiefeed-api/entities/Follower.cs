using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Foodiefeed_api.entities
{
    public class Follower
    {
        [Key]
        public int UserId { get; set; }
        [Key]
        public int FollowedUserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        [ForeignKey("FollowedUserId")]
        public virtual User FollowedUser { get; set; }
    }
}
