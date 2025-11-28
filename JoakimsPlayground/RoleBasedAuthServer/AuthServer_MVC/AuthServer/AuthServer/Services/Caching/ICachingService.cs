namespace AuthServer.Services.Caching
{
	public interface ICachingService
	{
		T? Get<T>(string key) where T : class;
		void Set<T>(string key, T value, TimeSpan expiration) where T : class;
		void Remove(string key);
	}
}
