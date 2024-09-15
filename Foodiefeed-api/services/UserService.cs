using AutoMapper;
using Foodiefeed_api.entities;
using Foodiefeed_api.exceptions;
using Foodiefeed_api.models.user;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace Foodiefeed_api.services
{
    public interface IUserService
    {
        public Task CreateUser(CreateUserDto dto);
        public Task<int> LogIn(UserLogInDto dto);
        public Task SetOnlineStatus(int id);
        public Task SetOfflineStatus(int id);

    }

    public class UserService : IUserService
    {
        private readonly dbContext _context;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher<User> _hasher;
        private readonly IEntityRepository<User> _entityRepository;

        public UserService(dbContext context,IMapper mapper, IPasswordHasher<User> hasher,IEntityRepository<User> entityRepository)
        {
            _context = context;
            _mapper = mapper;
            _hasher = hasher;
            _entityRepository = entityRepository;
        }

        public async Task CreateUser(CreateUserDto dto)
        {
            var userCheck = _context.Users.FirstOrDefault(u => u.Username == dto.Username); 

            if (userCheck is not null) { throw new BadRequestException("this username is taken"); }

            var emailCheck = _context.Users.FirstOrDefault(u => u.Email == dto.Email);

            if (emailCheck is not null) { throw new BadRequestException("this email is taken"); }

            var user = _mapper.Map<User>(dto);
            user.ProfilePicturePath = "default";


            user.PasswordHash = _hasher.HashPassword(user,user.PasswordHash);

            _context.Users.Add(user);

            await _context.SaveChangesAsync();

        }

        public async Task<int> LogIn(UserLogInDto dto)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == dto.Username);

            if (user is null) { throw new BadRequestException("User with that username do not exist"); }

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
    }
}
