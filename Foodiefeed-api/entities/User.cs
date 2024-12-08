namespace Foodiefeed_api.entities
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public bool IsOnline { get; set; } = false;

        public virtual ICollection<Friend> Friends { get; set; }
        public virtual ICollection<Post> Posts { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<UserTag> UserTags { get; set; }

        public virtual ICollection<FriendRequest> SendFriendRequests { get; set; }  //rename 
        public virtual ICollection<FriendRequest> ReceivedFriendRequests { get; set; }

        public virtual ICollection<Follower> Followers { get; set; }

        public virtual ICollection<CommentLike> CommentLikes { get; set; }
        public virtual ICollection<PostLike> PostLikes { get; set; }

        public virtual ICollection<Recipe> Recipes { get; set; }

    }
}