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
        public Task<List<ListedFriendDto>> GetOnlineFriends(int id, CancellationToken token);
        public Task<List<ListedFriendDto>> GetOfflineFriends(int id, CancellationToken token);

        public Task SendFriendRequest(int senderId, int reciverId);
        public Task AcceptFriendRequest(int senderId, int receiverId);
        public Task DeclineFriendRequest(int senderId, int receiverId);

        public Task<List<ListedFriendDto>> GetUserFriends(int userId, CancellationToken token);

        public Task CancelFriendRequest(int senderId,int receiverId);
        public Task Unfriend(int userId,int friendId);
    }

    public class FriendService : IFriendService
    {
        private readonly dbContext _dbContext;
        private readonly IEntityRepository<User> _userRepository;
        private readonly IMapper _mapper;
        private readonly INotificationService _notificationService;
        private readonly IAzureBlobStorageSerivce AzureBlobStorageService;

        private struct Status
        {
            public const bool Online = true;
            public const bool Offline = false;
        }

        private async Task Commit() => await _dbContext.SaveChangesAsync();

        public FriendService(dbContext dbContext,IEntityRepository<User> entityRepository,IMapper mapper,INotificationService notificationService,IAzureBlobStorageSerivce azureBlobStorageSerivce)
        {
            _dbContext = dbContext;
            _userRepository = entityRepository;
            _mapper = mapper;
            _notificationService = notificationService;
            AzureBlobStorageService = azureBlobStorageSerivce;
        }

        public async Task<List<ListedFriendDto>> GetOnlineFriends(int id, CancellationToken token)
        {
            return await GetFriendsByStatus(Status.Online,id, "User for whom trying to acces online freinds list do not exist",token);
        }

        public async Task<List<ListedFriendDto>> GetOfflineFriends(int id, CancellationToken token)
        {
            return await GetFriendsByStatus(Status.Offline, id, "User for whom trying to acces offline freinds list do not exist",token);
        }

        private async Task<List<ListedFriendDto>> GetFriendsByStatus(bool desiredStatus, int id,string message,CancellationToken token)
        {
            var user = _userRepository.FindById(id);

            if (user is null) { throw new NotFoundException(message); }

            token.ThrowIfCancellationRequested();

            var friends = await _dbContext.Friends.Where(f => f.UserId == user.Id || f.FriendUserId == user.Id).ToListAsync();

            List<ListedFriendDto> friendsList = new List<ListedFriendDto>();

            foreach (var friend in friends)
            {
                User? extractedFriendFromId;
                if (friend.UserId != id) 
                {
                    extractedFriendFromId = _userRepository.FindById(friend.UserId);
                }
                else
                {
                    extractedFriendFromId = _userRepository.FindById(friend.FriendUserId);
                }


                if (extractedFriendFromId is null) { break; } //user deleted his account, so he is non existant, can continue the operation skipping this entity.

                if (extractedFriendFromId.IsOnline == desiredStatus) {
                    var _friend = _mapper.Map<ListedFriendDto>(extractedFriendFromId);
                    var pfpStream = await AzureBlobStorageService.FetchProfileImageAsync(_friend.Id, token);
                    _friend.ProfilePictureBase64 = await AzureBlobStorageService.ConvertStreamToBase64Async(pfpStream,token);
                    friendsList.Add(_friend);
                }               
            }

            return friendsList;
        }

        public async Task SendFriendRequest(int senderId, int receiverId)
        {
            var friend = await _dbContext.Friends.FirstOrDefaultAsync(fr => (fr.UserId == senderId && fr.FriendUserId == receiverId)
                                                                         || (fr.UserId == receiverId && fr.FriendUserId == senderId));


            if (friend is not null) { throw new BadRequestException("This user already is your friend"); }

            var friendRequest = await _dbContext.FriendRequests.FirstOrDefaultAsync(fr => fr.SenderId == senderId && fr.ReceiverId == receiverId);
            var reflectedFriendRequest = await _dbContext.FriendRequests.FirstOrDefaultAsync(fr => fr.SenderId == receiverId && fr.ReceiverId == senderId);

            if (friendRequest is not null || reflectedFriendRequest is not null) { throw new BadRequestException("Request already sent"); }

            var username = _dbContext.Users.FirstOrDefault(u => u.Id == senderId).Username;

            await _notificationService.CreateNotification(NotificationType.FriendRequest,senderId,receiverId,username);
            _dbContext.FriendRequests.Add(new FriendRequest() { ReceiverId = receiverId,SenderId = senderId});

            await Commit();
        }

        public async Task CancelFriendRequest(int senderId,int receiverId)
        {
            var friendRequest = await _dbContext.FriendRequests
                .FirstOrDefaultAsync(fr => fr.SenderId == senderId && fr.ReceiverId == receiverId);
            
            if (friendRequest is null) { throw new NotFoundException("Such a friend request do not exist"); }

            var notfication = await _dbContext.Notifications.FirstOrDefaultAsync
                (fr => fr.ReceiverId == receiverId && 
                 fr.SenderId == senderId && 
                 fr.Type == NotificationType.FriendRequest);

            if (notfication is null) { throw new NotFoundException("Request do not exist in current context."); }
            _dbContext.Notifications.Remove(notfication);


            _dbContext.FriendRequests.Remove(friendRequest);

            await Commit();
        }

        public async Task AcceptFriendRequest(int senderId, int receiverId)
        {
            var friendRequest = await _dbContext.FriendRequests.FirstOrDefaultAsync(fr => fr.SenderId == senderId && fr.ReceiverId == receiverId);

            if(friendRequest is  null) { throw new NotFoundException("There is no such request"); }

            var notification = await _dbContext.Notifications.FirstOrDefaultAsync(n => n.SenderId == senderId && n.ReceiverId == receiverId && n.Type == NotificationType.FriendRequest);

            if(notification is null) { throw new NotFoundException("???"); }

            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == receiverId);
            if (user is null) { 
                _dbContext.Notifications.Remove(notification); // if user do not exist the notification should not exist either.
                throw new NotFoundException("User that send u firend request does not exist in current context."); 
            }

            await _notificationService.CreateNotification(NotificationType.AcceptedFriendRequest, receiverId, senderId, user.Username);

            _dbContext.Notifications.Remove(notification);
            _dbContext.FriendRequests.Remove(friendRequest);
            _dbContext.Friends.Add(new Friend() { UserId = senderId, FriendUserId = receiverId });

            await Commit();
        }

        public async Task DeclineFriendRequest(int senderId, int receiverId)
        {
            var friendRequest = await _dbContext.FriendRequests.FirstOrDefaultAsync(fr => fr.SenderId == senderId && fr.ReceiverId == receiverId);

            if(friendRequest is null) { throw new NotFoundException("No request to decline. This friend request do not exist in current context."); }

            var notification =  await _dbContext.Notifications.FirstOrDefaultAsync(n => n.SenderId == senderId && n.ReceiverId == receiverId && n.Type == NotificationType.FriendRequest) ;

            if(notification is null) { throw new NotFoundException("Notification could not be removed cause it do not exist in current context."); }
            _dbContext.Notifications.Remove(notification);
            _dbContext.FriendRequests.Remove(friendRequest);

            await Commit();
        }

        public async Task<List<ListedFriendDto>> GetUserFriends(int userId, CancellationToken token)
        {
            var user = _dbContext.Users.Include(u => u.Friends).FirstOrDefault(u => u.Id == userId);

            if(user is null) { throw new NotFoundException("user do not exist in current context."); }

            token.ThrowIfCancellationRequested();

            var friends = user.Friends.ToList();
            friends.AddRange(_dbContext.Friends.Where(f => f.FriendUserId == userId).ToList());
            List<int> FriendsId = new List<int>();

            token.ThrowIfCancellationRequested();

            foreach (var friend in friends)
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

            token.ThrowIfCancellationRequested();

            foreach (var friend in friendsAsUserList)
            {
                var imgStream = await AzureBlobStorageService.FetchProfileImageAsync(friend.Id, token);
                friendsAsUserListDto[i].ProfilePictureBase64 = await AzureBlobStorageService.ConvertStreamToBase64Async(imgStream, token);

                i++;
            }

            return friendsAsUserListDto;
        }

        public async Task Unfriend(int userId,int friendId)
        {
            var friend = await _dbContext.Friends.FirstOrDefaultAsync(fr => (fr.UserId == userId && fr.FriendUserId == friendId) 
                                                                         || (fr.UserId == friendId && fr.FriendUserId == userId));

            if (friend is null) { throw new NotFoundException("Request already sent"); }

            _dbContext.Remove(friend);
            await Commit();

        }

    }
}
