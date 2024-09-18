namespace Foodiefeed_api.models.posts
{
    public class PostDto
    {
        public int PostId { get; set; }
        public int UserId { get; set; }
        public required string Description { get; set; }
        public int Likes { get; set; }

        public required List<string> PostImagesBase64 { get; set; }
        public required List<string> ProductsName {  get; set; }
    }
}
