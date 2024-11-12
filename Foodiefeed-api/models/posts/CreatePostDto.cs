namespace Foodiefeed_api.models.posts
{
    public class CreatePostDto
    {
        public int UserId { get; set; }
        public string Description { get; set; }

        public List<IFormFile> Images {  get; set; } 
        public List<int> TagsId { get; set; }
        public List<int> ProductsId { get; set; }
    }
}
