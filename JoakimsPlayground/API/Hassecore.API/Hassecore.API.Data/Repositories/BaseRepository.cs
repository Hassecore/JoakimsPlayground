using Hassecore.API.Data.Context;
using Hassecore.API.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Hassecore.API.Data.Repositories
{
    public class BaseRepository : IBaseRepository
    {
        private readonly HassecoreApiContext _context;

        public BaseRepository(HassecoreApiContext context)
        {
            _context = context;
        }

        public async Task<T?> GetAsync<T>(Guid id) where T : class, IEntityBase
        {
            var user = await _context.Set<T>().FirstOrDefaultAsync<T>(x => x.Id == id);

            return user;
        }

        public async Task<T?> GetSingleOrDefaultAsync<T>(Expression<Func<T, bool>> predicate) where T : class, IEntityBase
        {
            IQueryable<T> query = _context.Set<T>();

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            return (await query.ToListAsync()).SingleOrDefault();
        }

        public async Task<List<T>> GetAsync<T>(Expression<Func<T, bool>> predicate) where T : class, IEntityBase
        {
            IQueryable<T> query = _context.Set<T>();

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            return await query.ToListAsync();
        }

        public async Task<T> CreateAsync<T>(T entity) where T : class, IEntityBase
        {
            _context.Add(entity);
            await _context.SaveChangesAsync();

            return entity;
        }
    }
}
