namespace Foodiefeed_api.models.user
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public Uri ProfilePictureBase64 { get; set; }
        public int FollowersCount { get; set; }
        public int FriendsCount { get; set; }
    }
}
