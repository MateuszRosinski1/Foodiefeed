namespace Foodiefeed
{
    public class Post
    {
        // pfp
        public string PosterUsername { get; set; }
        public bool isFollowed { get; set; }
        public string PostTextContent { get; set; }
        public List<Image> Images { get; set; }
        public int Likes { get; set; }
        public bool isLiked { get; set; }
        public List<Comment> CommentSection { get; set; }
    }

    public class Comment
    {
        public string CommentPosterUsername { get; set; }
        public bool isFollowed { get; set; }
        public string CommentTextContent { get; set; }
        public int Likes { get; set; }
        public bool isLiked { get; set; }
    }
}
