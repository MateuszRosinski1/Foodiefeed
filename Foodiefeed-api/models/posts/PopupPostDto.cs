﻿namespace Foodiefeed_api.models.posts
{
    public class PopupPostDto
    {

        public int PostId { get; set; }
        public int UserId { get; set; }
        public required string Username { get; set; }
        public required string Description { get; set; }
        public int Likes { get; set; }
        public string PosterProfilePictureBase64 { get; set; }

        public required List<string> PostImagesBase64 { get; set; }
        public required List<string> ProductsName { get; set; }

        public string CommentProfilePictureImageBase64 { get; set; }
        public required string CommentUsername { get; set; }
        public required string CommentContent { get; set; }
        public required string CommentUserId { get; set; }
        public required string CommentLikes { get; set; }

    }
}