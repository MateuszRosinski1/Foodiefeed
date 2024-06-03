using Foodiefeed_api.entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Foodiefeed_api.models.PostTag
{
    public class PostTagDto
    {
        public int PostId { get; set; }
        public string TagName { get; set; }
        public string Description { get; set; }
    }
}
