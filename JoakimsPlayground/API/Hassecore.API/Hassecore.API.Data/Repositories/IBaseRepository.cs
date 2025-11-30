using Hassecore.API.Data.Entities;
using System.Linq.Expressions;

namespace Hassecore.API.Data.Repositories
{
    public interface IBaseRepository
    {
        Task<T?> GetAsync<T>(Guid id) where T : class, IEntityBase;
        Task<T?> GetSingleOrDefaultAsync<T>(Expression<Func<T, bool>> predicate) where T : class, IEntityBase;
        Task<List<T>> GetAsync<T>(Expression<Func<T, bool>> predicate) where T : class, IEntityBase;
        Task<T> CreateAsync<T>(T entity) where T: class, IEntityBase;
    }
}
