using Foodiefeed_api.entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Foodiefeed_api.models.posts
{
    public class CreatePostDto
    {
        public int PostId { get; set; }
        public int UserId { get; set; }
        public string Description { get; set; }

        public List<string> ImagesBase64 { get; set; }
        public List<int> TagIds { get; set; }

        //public virtual ICollection<PostProduct> PostProducts { get; set; }
    }
}
