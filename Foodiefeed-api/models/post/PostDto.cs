using Foodiefeed_api.entities;
using Foodiefeed_api.models.comment;
using Foodiefeed_api.models.PostTag;

namespace Foodiefeed_api.models.post
{
    public class PostDto
    {
        public int PostId { get; set; }
        public int UserId { get; set; }
        public string Description { get; set; }
        public int Likes { get; set; }
        public List<string> imagePath { get; set; }
        public List<CommentDto> Comments { get; set; }
        public List<PostTagDto> Tags { get; set; }

        //public virtual ICollection<PostProduct> PostProducts { get; set; } need a huge refactor, 
    }
}
