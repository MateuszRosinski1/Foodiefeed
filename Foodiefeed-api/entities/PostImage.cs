using System.ComponentModel.DataAnnotations.Schema;

namespace Foodiefeed_api.entities
{
    public class PostImage
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public string ImagePath { get; set; }

        [ForeignKey("PostId")]
        public virtual Post Post { get; set; }
    }
}