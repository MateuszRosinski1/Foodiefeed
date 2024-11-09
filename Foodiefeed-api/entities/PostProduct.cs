using System.ComponentModel.DataAnnotations.Schema;

namespace Foodiefeed_api.entities
{
    public class PostProduct
    {
        public int PostProductId { get; set; }
        public int PostId { get; set; }
        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public virtual Products Product { get; set; }
    }
}