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

        public Task<List<ListedFriendDto>> GetUserFriends(int userId);
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
                else
                {
                    extractedFriendFromId = _userRepository.FindById(friend.FriendUserId); // 3 1
                }


                if (extractedFriendFromId is null) { break; } // smth do to with this

                if (extractedFriendFromId.IsOnline == desiredStatus) {
                    var onlinefriend = _mapper.Map<ListedFriendDto>(extractedFriendFromId);
                    friendsList.Add(onlinefriend);
                }

                
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

        public async Task CancelFriendRequest(int senderId,int receiverId)
        {
            var friendRequest = await _dbContext.FriendRequests.FirstOrDefaultAsync(fr => fr.SenderId == senderId && fr.ReceiverId == receiverId);
            
            if (friendRequest is null) { throw new NotFoundException("Such a friend request do not exist"); }

            _dbContext.FriendRequests.Remove(friendRequest);

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

        public async Task<List<ListedFriendDto>> GetUserFriends(int userId)
        {
            var user = _dbContext.Users.Include(u => u.Friends).FirstOrDefault(u => u.Id == userId);

            if(user is null) { throw new NotFoundException("user do not exist in current context."); }

            var friends = user.Friends.ToList();
            friends.AddRange(_dbContext.Friends.Where(f => f.FriendUserId == userId).ToList());
            List<int> FriendsId = new List<int>();

            foreach(var friend in friends)
            {
                if(friend.FriendUserId == userId)
                {
                    FriendsId.Add(friend.UserId);
                }
                else
                {
                    FriendsId.Add(friend.FriendUserId);
                }
            }

            var friendsAsUserList = _dbContext.Users.Where(u => FriendsId.Contains(u.Id)).ToList();

            var friendsAsUserListDto = _mapper.Map<List<ListedFriendDto>>(friendsAsUserList);
            int i = 0;
            foreach (var friend in friendsAsUserList)
            {
                //friendsAsUserListDto[i].ProfilePictureBase64 = "iVBORw0KGgoAAAANSUhEUgAAAMgAAADICAIAAAAiOjnJAAAAAXNSR0IArs4c6QAAAIpJREFUeJztwQEBAAAAgiD/r25IQAEAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA8GDVlwABvawfoQAAAABJRU5ErkJggg==";
                friendsAsUserListDto[i].ProfilePictureBase64 = "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";
                i++;
            }

            return friendsAsUserListDto;
        }
    }
}
