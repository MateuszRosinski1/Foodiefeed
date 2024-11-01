using Foodiefeed_api.models.comment;

namespace Foodiefeed_api.models.posts
{
    public class PostDto
    {
        public int PostId { get; set; }
        public int UserId { get; set; } 
        public string ProfilePictureBase64 { get; set; }
        public required string Username { get; set; }
        public required string Description { get; set; }
        public int Likes { get; set; }
        public string TimeSpan { get; set; }

        public required List<string> PostImagesBase64 { get; set; }
        public required List<string> ProductsName {  get; set; }

        public required List<CommentDto> Comments { get; set; }

        public void ConvertDateTimeToTimeSpan(DateTime dateTime) {

            TimeSpan = string.Empty;
        }
    }
}
