using AutoMapper;
using Foodiefeed_api.models.friends;
using Foodiefeed_api.entities;
using Microsoft.EntityFrameworkCore;
using Foodiefeed_api.exceptions;

namespace Foodiefeed_api.services
{
    public interface IFollowerService
    {
        public Task<List<ListedFriendDto>> GetFollowerListAsync(int id, CancellationToken token);
        public Task Follow(int userId, int followedUserId);
        public Task Unfollow(int userId, int unfollowedUserId);
    }

    public class FollowerService : IFollowerService
    {
        private readonly dbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly INotificationService _notificationService;
        private readonly IAzureBlobStorageSerivce AzureBlobStorageService;


        public FollowerService(dbContext dbContext,IMapper mapper,INotificationService notificationService,IAzureBlobStorageSerivce azureBlobStorageService)
        {
            _dbContext = dbContext;
            _mapper = mapper;         
            _notificationService = notificationService;
            AzureBlobStorageService = azureBlobStorageService;
        }

        private async Task Commit()
        {
            await _dbContext.SaveChangesAsync();
        }

        public async Task Follow(int userId, int followedUserId)
        {
            var follower = await _dbContext.Followers.FirstOrDefaultAsync(f => f.UserId == userId && f.FollowedUserId == followedUserId);

            if(follower is not null) { throw new BadRequestException("user is already followed."); }

            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if(user is null) { throw new BadRequestException($"user with id:{userId} do not exist in current context"); }

            await _notificationService.CreateNotification(NotificationType.GainFollower,userId,followedUserId,user.Username);
            _dbContext.Followers.Add(new Follower() { UserId = userId, FollowedUserId = followedUserId });
            await Commit();
        }


        public async Task Unfollow(int userId, int unfollowedUserId)
        {
            var follower = await _dbContext.Followers.FirstOrDefaultAsync(f => f.UserId == userId && f.FollowedUserId == unfollowedUserId);

            if (follower is null) { throw new BadRequestException("user is not follwed"); }

            var notification = await _dbContext.Notifications.FirstOrDefaultAsync(u => u.Type == NotificationType.GainFollower &&
            u.SenderId == userId &&
            u.ReceiverId == unfollowedUserId);

            if(notification is null) { throw new NotFoundException("notification do not exist in current context."); }

            _dbContext.Notifications.Remove(notification);
            _dbContext.Followers.Remove(follower);
            await Commit();
        }

        public async Task<List<ListedFriendDto>> GetFollowerListAsync(int id, CancellationToken token)
        {
            var followers = await _dbContext.Followers.
                Where(f => f.FollowedUserId == id).
                ToListAsync();

            var userModels = new List<User>();

            token.ThrowIfCancellationRequested();

            foreach (var follower in followers)
            {
                var user = await _dbContext.Users
                    .FirstOrDefaultAsync(u => u.Id == follower.UserId);

                if (user is null)
                {
                    break;
                }

                userModels.Add(user);
            }

            var userModelDtos = _mapper.Map<List<ListedFriendDto>>(userModels);

            token.ThrowIfCancellationRequested();

            foreach (var dto in userModelDtos)
            {
                //var imgStream = await AzureBlobStorageService.FetchProfileImageAsync(dto.Id,token);
                dto.ProfilePictureBase64 = await AzureBlobStorageService.FetchProfileImage(dto.Id, token);

            }
            //reusing ListedFriendDto as ListedFollowerDto

            return userModelDtos;
        }
    }
}
