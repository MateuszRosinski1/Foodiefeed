using System.ComponentModel.DataAnnotations;

namespace Foodiefeed.models.dto
{
    class AddPostDto
    {
        public int UserId { get; set; }
        public string Description { get; set; } // post content

        [MaxLength(4)]
        public List<int> TagsId { get; set; }
        
        
    }
}
