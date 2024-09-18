using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foodiefeed.models.dto
{
    public class PostDto
    {
        public int PostId { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; }
        public required string TimeStamp { get; set; }
        public required string Description { get; set; }
        public int Likes { get; set; }

        public required List<string> PostImagesBase64 { get; set; }
        public required List<string> ProductsName { get; set; }
    }
}
