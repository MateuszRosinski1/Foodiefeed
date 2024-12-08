namespace Foodiefeed_api.models.comment
{
    public class CommentDto
    {
        public int CommentId { get; set; }
        public string CommentContent { get; set; }
        public int Likes { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; }
        public string ImageBase64 { get; set; }
        public bool IsLiked { get; set; }
    }
}
