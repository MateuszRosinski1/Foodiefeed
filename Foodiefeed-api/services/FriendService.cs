using AutoMapper;
using Foodiefeed_api.entities;
using Foodiefeed_api.models.friends;
using Foodiefeed_api.exceptions;

namespace Foodiefeed_api.services
{
    public interface IFriendService
    {
        public Task<List<OnlineFriendDto>> GetOnlineFriends(int id);
    }

    public class FriendService : IFriendService
    {
        private readonly dbContext _dbContext;
        private readonly IEntityRepository<User> _userRepository;
        private readonly IMapper _mapper;

        public FriendService(dbContext dbContext,IEntityRepository<User> entityRepository,IMapper mapper)
        {
            _dbContext = dbContext;
            _userRepository = entityRepository;
            _mapper = mapper;
        }

        public async Task<List<OnlineFriendDto>> GetOnlineFriends(int id)
        {
            var user = _userRepository.FindById(id);

            if (user is null) { throw new NotFoundException("User for whom trying to acces online freinds list do not exist"); }// custom error here

            var friends = user.Friends;
            List<OnlineFriendDto> onlineFriends = new List<OnlineFriendDto>();

            foreach (var friend in friends)
            {
                var extractedFriendFromId = _userRepository.FindById(friend.FriendUserId);
                if (extractedFriendFromId is null) {  break; } // smth do to with this

                if(extractedFriendFromId.IsOnline!) { break; }

                var onlinefriend = _mapper.Map<OnlineFriendDto>(extractedFriendFromId);
                onlineFriends.Add(onlinefriend);
            }

            return onlineFriends;
        }

        public async Task GetOfflineFriends(int id)
        {

        }
    }
}
