using System.Runtime.Caching;

namespace AuthServer.Services.Caching
{
	public class CachingService : ICachingService
	{
		private static readonly MemoryCache _cache = MemoryCache.Default;

		public T? Get<T>(string key) where T : class
		{
			return _cache.Get(key) as T;
		}

		public void Set<T>(string key, T value, TimeSpan expiration) where T : class
		{
			if (Get<T>(key) != null)
			{
				_cache.Remove(key);
			}

			_cache.Set(key, value, DateTimeOffset.Now.Add(expiration));
		}

		public void Remove(string key)
		{
			_cache.Remove(key);
		}
	}
}
