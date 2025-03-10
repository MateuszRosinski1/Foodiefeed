﻿using AutoMapper;
using Foodiefeed_api.entities;
using Foodiefeed_api.exceptions;
using Foodiefeed_api.models.user;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using FuzzySharp;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Foodiefeed_api.services
{
    public interface IUserService
    {
        public Task CreateUser(CreateUserDto dto,IFormFile file, CancellationToken token);
        public Task<int> LogIn(UserLogInDto dto, CancellationToken token);
        public Task SetOnlineStatus(int id);
        public Task SetOfflineStatus(int id);
        public Task<List<UserDto>> SearchUsers(string usernameQuery, string userId, CancellationToken token);
        public Task<UserDto> GetById(string id, CancellationToken token);
        public Task<UserProfileModel> GetUserProfileModelAsync(string id, string askerId, CancellationToken token);

        public Task ChangeUsername(int id, string value);
        public Task ChangeEmail(int id, string value);
        public Task ChangePassword(int id, string value);
        public Task ChangeProfilePicture(int id,IFormFile file);
        public Task RemoveProfilePicture(int userId);

        public Task<string> GetProfilePicture(int userId, CancellationToken token);
    }

    public class UserService : IUserService
    {
        private readonly dbContext _context;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher<User> _hasher;
        private readonly IEntityRepository<User> _entityRepository;
        private readonly IFriendService _friendService;
        private readonly IAzureBlobStorageSerivce AzureBlobStorageService;

        public UserService(dbContext context,
            IMapper mapper, 
            IPasswordHasher<User> hasher,
            IEntityRepository<User> entityRepository,
            IFriendService friendService,
            IFollowerService followerService,
            IAzureBlobStorageSerivce _azureBlobStorageSerivce)
        {
            _context = context;
            _mapper = mapper;
            _hasher = hasher;
            _entityRepository = entityRepository;
            _friendService = friendService;
            AzureBlobStorageService = _azureBlobStorageSerivce;
        }

        public async Task<List<UserDto>> SearchUsers(string usernameQuery,string userId, CancellationToken token)
        {
           var users = await _context.Users
                .Where(u => u.Username.Contains(usernameQuery.Substring(0, 1)) && u.Id != Convert.ToInt32(userId))
                .ToListAsync();

            token.ThrowIfCancellationRequested();

            var searchedUsers = users.Where(u =>
                u.Username.StartsWith(usernameQuery, StringComparison.OrdinalIgnoreCase) 
                || Fuzz.Ratio(u.Username.ToLower(), usernameQuery.ToLower()) >= 85).Take(20);

            token.ThrowIfCancellationRequested();

            var usersDto = _mapper.Map<List<UserDto>>(searchedUsers);
            
            foreach (var userDto in usersDto)
            {
                userDto.FriendsCount = await GetUserFriendsCount(userDto.Id,token);
                userDto.FollowersCount = await GetUserFollowersCount(userDto.Id, token);              
                userDto.ProfilePictureBase64 = await AzureBlobStorageService.FetchProfileImage(userDto.Id,token);
            }
            return usersDto;
        }

        public async Task CreateUser(CreateUserDto dto, IFormFile file, CancellationToken token)
        {
            var userCheck = _context.Users.FirstOrDefault(u => u.Username == dto.Username); 

            if (userCheck is not null) { throw new BadRequestException("this username is taken"); }

            var emailCheck = _context.Users.FirstOrDefault(u => u.Email == dto.Email);

            if (emailCheck is not null) { throw new BadRequestException("this email is taken"); }

            var user = _mapper.Map<User>(dto);

            user.PasswordHash = _hasher.HashPassword(user,user.PasswordHash);

            List<UserTag> userTags = new List<UserTag>();

            token.ThrowIfCancellationRequested();

            foreach (var tag in _context.Tags.ToList())
            {
                userTags.Add(new UserTag() { Score = 50, Tag = tag });
            }

            user.UserTags = userTags;
            _context.Users.Add(user);

            await _context.SaveChangesAsync();

            await ChangeProfilePicture(user.Id, file);

        }

        public async Task<int> LogIn(UserLogInDto dto, CancellationToken token)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);

            if (user is null) { throw new BadRequestException("User with that username do not exist"); }

            token.ThrowIfCancellationRequested();

            var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
            
            if(result == PasswordVerificationResult.Failed) { throw new BadRequestException("Wrong password"); }

            return user.Id;           
        }     

        public async Task SetOnlineStatus(int id)
        {
            var user = _entityRepository.FindById(id);

            if (user is null) { return; }  //notfound here custom exepction.

            user.IsOnline = true;
            await _context.SaveChangesAsync();
        }

        public async Task SetOfflineStatus(int id)
        {
            var user = _entityRepository.FindById(id);

            if (user is null) { return; }  //notfound here custom exepction.

            user.IsOnline = false;
            await _context.SaveChangesAsync();
        }

        public async Task<UserProfileModel> GetUserProfileModelAsync(string id,string askerId, CancellationToken token)
        {
            var user = _entityRepository.FindById(Convert.ToInt32(id));

            if (user is null) { throw new NotFoundException($"User with id {id} not found"); }

            var userProfileModel =  _mapper.Map<UserProfileModel>(user);

            var friendsCount = await GetUserFriendsCount(userProfileModel.Id, token);
            var followersCount = await GetUserFollowersCount(userProfileModel.Id, token);

            token.ThrowIfCancellationRequested();

            if (id != askerId)
            {
                var asker = await _context.Users.Include(u => u.Followers)
                                                .FirstOrDefaultAsync(x => x.Id == Convert.ToInt32(askerId));

                if (asker is null) { throw new NotFoundException(""); }

                var friend = await _context.Friends.FirstOrDefaultAsync
                    (x => x.FriendUserId == Convert.ToInt32(askerId) && x.UserId == Convert.ToInt32(id) ||
                    (x.FriendUserId == Convert.ToInt32(id) && x.UserId == Convert.ToInt32(askerId)));

                var follower = asker.Followers.FirstOrDefault(f => f.FollowedUserId == Convert.ToInt32(id));

                var friendRequest = await _context.FriendRequests
                .FirstOrDefaultAsync(fr => (fr.SenderId == Convert.ToInt32(askerId) && fr.ReceiverId == Convert.ToInt32(id)));

                userProfileModel.IsFriend = friend is null ? false : true;
                userProfileModel.IsFollowed = follower is null ? false : true;
                userProfileModel.HasPendingFriendRequest = friendRequest is null ? false : true;
            }

            userProfileModel.FriendsCount = friendsCount.ToString();
            userProfileModel.FollowsCount = followersCount.ToString();

            //var pfpStream = await AzureBlobStorageService.FetchProfileImageAsync(userProfileModel.Id,token);
            userProfileModel.ProfilePictureBase64 = await AzureBlobStorageService.FetchProfileImage(userProfileModel.Id, token);

            return userProfileModel;
        }

        private async Task<int> GetUserFollowersCount(int id,CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var followers = _context.Followers.Where(f => f.FollowedUserId == id);

            return followers.Count();
        }

        private async Task<int> GetUserFriendsCount(int id, CancellationToken token)
        {
            var onlineFriends = await _friendService.GetOnlineFriends(id, token);
            var offlineFriends = await _friendService.GetOfflineFriends(id, token);

            return onlineFriends.Count() + offlineFriends.Count();
        }

        public async Task<UserDto> GetById(string id, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var user =  _entityRepository.FindById(Convert.ToInt32(id));

            if(user is null) { throw new NotFoundException("user not found"); }

            var userDto = _mapper.Map<UserDto>(user);
            userDto.FollowersCount = await GetUserFollowersCount(user.Id, token);
            userDto.FriendsCount = await GetUserFriendsCount(user.Id, token);
            userDto.ProfilePictureBase64 = await AzureBlobStorageService.FetchProfileImage(userDto.Id,token);


            return userDto;

        }

        public async Task ChangeUsername(int id,string value)
        {
            var user = await _entityRepository.FindByIdAsync(id);

            if(user is null) { throw new NotFoundException($"User with {id} do not exist in current context"); }

            user.Username = value;

            _context.Users.Update(user);
            _context.SaveChanges();
        }

        public async Task ChangeEmail(int id, string value)
        {
            var user = await _entityRepository.FindByIdAsync(id);

            if (user is null) { throw new NotFoundException($"User with {id} do not exist in current context"); }

            user.Email = value;

            _context.Users.Update(user);
            _context.SaveChanges();
        }

        public async Task ChangePassword(int id, string value)
        {
            var user = await _entityRepository.FindByIdAsync(id);

            if (user is null) { throw new NotFoundException($"User with {id} do not exist in current context"); }

            user.PasswordHash = _hasher.HashPassword(user,value);

            _context.Users.Update(user);
            _context.SaveChanges();
        }

        public async Task<string> GetProfilePicture(int userId, CancellationToken token)
        {
            var uri = await AzureBlobStorageService.FetchProfileImage(userId,token);

            if (uri is null) { throw new NotFoundException("user uses the deafault profile picture."); }

            //var base64 = await AzureBlobStorageService.ConvertStreamToBase64Async(stream, token);

            return uri.ToString();
        }

        public async Task ChangeProfilePicture(int id, IFormFile file)
        {
            await AzureBlobStorageService.UploadNewProfilePicture(id, file);
        }

        public async Task RemoveProfilePicture(int userId)
        {
            await AzureBlobStorageService.RemoveUserProfilePicture(userId);
        }
    }
}
