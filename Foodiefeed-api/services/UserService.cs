using AutoMapper;
using Foodiefeed_api.entities;
using Foodiefeed_api.models.user;

namespace Foodiefeed_api.services
{
    public interface IUserService
    {
        Task CreateUser(CreateUserDto dto);
    }
    public class UserService : IUserService
    {
        private readonly dbContext _context;
        private readonly IMapper _mapper;
        public UserService(dbContext context,IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task CreateUser(CreateUserDto dto)
        {
            var user = _mapper.Map<User>(dto);

            _context.Users.Add(user);

            await _context.SaveChangesAsync();

        }

    }
}
