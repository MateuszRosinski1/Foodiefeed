using System.ComponentModel.DataAnnotations.Schema;

namespace Foodiefeed_api.entities
{
    public class PostProduct
    {
        public int PostProductId { get; set; }
        public int PostId { get; set; }
        public string Product { get; set; }
        //public virtual Post Post { get; set; }
    }
}