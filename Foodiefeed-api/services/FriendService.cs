using AutoMapper;
using Foodiefeed_api.entities;
using Foodiefeed_api.models.friends;
using Foodiefeed_api.exceptions;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;

namespace Foodiefeed_api.services
{
    public interface IFriendService
    {
        public Task<List<ListedFriendDto>> GetOnlineFriends(int id);
        public Task<List<ListedFriendDto>> GetOfflineFriends(int id);

        public Task SendFriendRequest(int senderId, int reciverId);
        public Task AcceptFriendRequest(int senderId, int receiverId);
        public Task DeclineFriendRequest(int senderId, int receiverId);
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

        private async Task Commit() => await _dbContext.SaveChangesAsync();

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

            var friends = await _dbContext.Friends.Where(f => f.UserId == user.Id || f.FriendUserId == user.Id).ToListAsync();

            List<ListedFriendDto> friendsList = new List<ListedFriendDto>();

            foreach (var friend in friends)
            {
                User? extractedFriendFromId;
                if (friend.UserId != id) 
                {
                    extractedFriendFromId = _userRepository.FindById(friend.UserId);  //  1 2
                }
                extractedFriendFromId = _userRepository.FindById(friend.FriendUserId); // 3 1


                if (extractedFriendFromId is null) { break; } // smth do to with this

                if (extractedFriendFromId.IsOnline != desiredStatus) { break; }

                var onlinefriend = _mapper.Map<ListedFriendDto>(extractedFriendFromId);
                friendsList.Add(onlinefriend);
            }

            return friendsList;
        }

        public async Task SendFriendRequest(int senderId, int receiverId)
        {
            var friendRequest = await _dbContext.FriendRequests.FirstOrDefaultAsync(fr => fr.SenderId == senderId && fr.ReceiverId == receiverId);
            var reflectedFriendRequest = await _dbContext.FriendRequests.FirstOrDefaultAsync(fr => fr.SenderId == receiverId && fr.ReceiverId == senderId);

            if (friendRequest is not null && reflectedFriendRequest is not null) { throw new Exception("Request already sent"); }

            _dbContext.FriendRequests.Add(new FriendRequest() { ReceiverId = receiverId,SenderId = senderId});

            await Commit();
        }

        public async Task AcceptFriendRequest(int senderId, int receiverId)
        {
            var friendRequest = await _dbContext.FriendRequests.FirstOrDefaultAsync(fr => fr.SenderId == senderId && fr.ReceiverId == receiverId);

            if(friendRequest is  null) { throw new NotFoundException("There is no such request"); }

            _dbContext.FriendRequests.Remove(friendRequest);
            _dbContext.Friends.Add(new Friend() { UserId = senderId, FriendUserId = receiverId });

            await Commit();
        }

        public async Task DeclineFriendRequest(int senderId, int receiverId)
        {
            var friendRequest = await _dbContext.FriendRequests.FirstOrDefaultAsync(fr => fr.SenderId == senderId && fr.ReceiverId == receiverId);

            if(friendRequest is null) { throw new NotFoundException("No request to decline. This friend request do not exist in current context."); }

            _dbContext.FriendRequests.Remove(friendRequest);

            await Commit();
        }
    }
}
