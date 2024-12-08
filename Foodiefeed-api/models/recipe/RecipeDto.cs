namespace Foodiefeed_api.models.recipe
{
    public class RecipeDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Content { get; set; }
        public List<string> Products { get; set; }
        public string ImageBase64 { get; set; }
        public int UserId { get; set; }
    }
}
