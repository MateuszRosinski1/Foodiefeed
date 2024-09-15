using Foodiefeed_api.entities;
using Microsoft.EntityFrameworkCore;

namespace Foodiefeed_api
{
    public class EntityRepository<T> : IEntityRepository<T> where T : class
    {
        private readonly dbContext _context;
        private readonly DbSet<T> _entity;

        public EntityRepository(dbContext context)
        {
            _context = context;
            _entity = context.Set<T>();
        }

        public T? FindById(int id)
        {
            var entity = _entity.Find(id);

            return entity;
        }
    }
}
