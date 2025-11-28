using JoakimsPlaygroundFunctions.Data.Entities;
using System.Linq.Expressions;

namespace JoakimsPlaygroundFunctions.Data.Repositories
{
	public interface IRepository
	{
		IEnumerable<T> GetEnumerable<T>(Expression<Func<T, bool>> predicate, string[]? includes = null) where T : EntityBase;

		T Get<T>(Expression<Func<T, bool>> predicate, string[]? includes = null) where T : EntityBase;

		Task<Guid> CreateAsync<T>(T entity) where T : EntityBase;
	}
}
