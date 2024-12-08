namespace Foodiefeed.models.dto;

public class PostDto
{
    public int PostId { get; set; }
    public int UserId { get; set; }
    public string Username { get; set; }
    public string ProfilePictureBase64 { get; set; }
    public string TimeSpan { get; set; }
    public required string Description { get; set; }
    public int Likes { get; set; }

    public required List<string> PostImagesBase64 { get; set; }
    public required List<string> ProductsName { get; set; }
    public required List<CommentDto> Comments { get; set; }

    public bool IsLiked { get; set; }
    public bool IsSaved { get; set; }
}
