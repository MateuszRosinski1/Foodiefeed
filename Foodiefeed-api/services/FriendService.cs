using AutoMapper;
using Foodiefeed_api.entities;
using Foodiefeed_api.models.friends;
using Foodiefeed_api.exceptions;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Foodiefeed_api.services
{
    public interface IFriendService
    {
        public Task<List<ListedFriendDto>> GetOnlineFriends(int id);
        public Task<List<ListedFriendDto>> GetOfflineFriends(int id);
    }

    public class FriendService : IFriendService
    {
        private readonly dbContext _dbContext;
        private readonly IEntityRepository<User> _userRepository;
        private readonly IMapper _mapper;

        private struct Status
        {
            public const bool Online = true;
            public const bool Offline = false;
        }

        public FriendService(dbContext dbContext,IEntityRepository<User> entityRepository,IMapper mapper)
        {
            _dbContext = dbContext;
            _userRepository = entityRepository;
            _mapper = mapper;
        }

        public async Task<List<ListedFriendDto>> GetOnlineFriends(int id)
        {
            return await GetFriendsByStatus(Status.Online,id, "User for whom trying to acces online freinds list do not exist");
        }

        public async Task<List<ListedFriendDto>> GetOfflineFriends(int id)
        {
            return await GetFriendsByStatus(Status.Offline, id, "User for whom trying to acces offline freinds list do not exist");
        }

        private async Task<List<ListedFriendDto>> GetFriendsByStatus(bool desiredStatus, int id,string message)
        {
            var user = _userRepository.FindById(id);

            if (user is null) { throw new NotFoundException(message); }// custom error here

            var friends = user.Friends;
            List<ListedFriendDto> friendsList = new List<ListedFriendDto>();

            foreach (var friend in friends)
            {
                var extractedFriendFromId = _userRepository.FindById(friend.FriendUserId);

                if (extractedFriendFromId is null) { break; } // smth do to with this

                if (extractedFriendFromId.IsOnline != desiredStatus) { break; }

                var onlinefriend = _mapper.Map<ListedFriendDto>(extractedFriendFromId);
                friendsList.Add(onlinefriend);
            }

            return friendsList;
        }
    }
}
