using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Foodiefeed_api.entities
{
    public class UserTag
    {
        [Key]
        public int UserId { get; set; }
        [Key]
        public int TagId { get; set; }
        [Range(0,100)]
        public int Score { get; set; } 

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        [ForeignKey("TagId")]
        public virtual Tag Tag { get; set; }
    }
}