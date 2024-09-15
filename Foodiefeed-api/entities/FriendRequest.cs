using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Foodiefeed_api.entities
{
    public class FriendRequest
    {
        [Key]
        public int SenderId { get; set; }
        [Key]
        public int ReceiverId { get; set; }

        [ForeignKey("SenderId")]
        public virtual User Sender { get; set; }
        [ForeignKey("ReceiverId")]
        public virtual User Receiver { get; set; }
    }
}
