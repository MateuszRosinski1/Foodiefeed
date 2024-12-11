namespace Foodiefeed_api.models.user
{
    public class UserProfileModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public Uri ProfilePictureBase64 { get; set; }
        public string FriendsCount { get; set; }
        public string FollowsCount { get; set; }
        public bool IsFollowed { get; set; }
        public bool IsFriend { get; set; }
        public bool HasPendingFriendRequest { get; set; }
    }
}
