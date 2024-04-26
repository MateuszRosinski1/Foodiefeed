using System.ComponentModel.DataAnnotations.Schema;

namespace Foodiefeed_api.entities
{
    public class UserTag
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string TagName { get; set; }
        public int Count { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}