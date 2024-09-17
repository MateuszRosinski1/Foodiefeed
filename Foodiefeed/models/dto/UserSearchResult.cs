using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foodiefeed.models.dto
{
    internal class UserSearchResult
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string ProfilePictureBase64 { get; set; }
        public int FollowersCount { get; set; }
        public int FriendsCount { get; set; }
    }
}
