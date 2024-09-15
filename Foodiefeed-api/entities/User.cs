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
        public string ProfilePicturePath { get; set; }
        public bool IsOnline { get; set; } = false;

        public virtual ICollection<Friend> Friends { get; set; }
        public virtual ICollection<Post> Posts { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<UserTag> UserTags { get; set; }

        public virtual ICollection<FriendRequest> SentFriendRequests { get; set; }
        public virtual ICollection<FriendRequest> ReceivedFriendRequests { get; set; }

    }
}