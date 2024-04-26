using System.ComponentModel.DataAnnotations.Schema;

namespace Foodiefeed_api.entities
{
    public class PostTag
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public string TagName { get; set; }
        public string Description { get; set; }

        [ForeignKey("PostId")]
        public virtual Post Post { get; set; }
    }
}