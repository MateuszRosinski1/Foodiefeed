using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foodiefeed.models.dto
{
    public class RecipeDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Content { get; set; }
        public List<string> Products { get; set; }
        public string ImageBase64 { get; set; }
    }
}
