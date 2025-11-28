using JoakimsPlaygroundFunctions.Data.Contexts;
using JoakimsPlaygroundFunctions.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace JoakimsPlaygroundFunctions.Data.Repositories
{
	public class Repository : IRepository
	{
		protected readonly Context _context;

		public Repository(Context context)
		{
			_context = context;
		}

		public IEnumerable<T> GetEnumerable<T>(Expression<Func<T, bool>> predicate, string[]? includes = null) where T : EntityBase
		{
			var query = _context.Set<T>().AsQueryable();

			if (includes?.Length > 0)
			{
				foreach (var include in includes)
				{
					query = query.Include(include);
				}
			}

			return query.Where(predicate);
		}

		public T Get<T>(Expression<Func<T, bool>> predicate, string[]? includes = null) where T : EntityBase
		{
			IQueryable<T> query = _context.Set<T>();

			if (includes != null)
			{
				foreach (var include in includes)
				{
					query = query.Include(include);
				}
			}

			var entity = query.SingleOrDefault(predicate);

			if (entity == null)
			{
				throw new InvalidDataException($"Entity of type {typeof(T).Name} not found.");
			}

			return entity;
		}

		public async Task<Guid> CreateAsync<T>(T entity) where T : EntityBase
		{
			if (entity == null)
			{
				throw new ArgumentNullException(nameof(entity));
			}
			_context.Set<T>().Add(entity);
			await _context.SaveChangesAsync();
			return entity.Id;
		}
	}
}
