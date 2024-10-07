using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Foodiefeed_api.entities
{
    public class Friend
    {
        [Key]
        public int UserId { get; set; }
        [Key]
        public int FriendUserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        [ForeignKey("FriendUserId")]
        public virtual User FriendUser { get; set; }
    }
}