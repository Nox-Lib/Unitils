using System.Collections.Generic;

namespace Unitils
{
	public class MemoryCache
	{
		private readonly Dictionary<string, object> cache;

		public MemoryCache()
		{
			this.cache = new Dictionary<string, object>();
		}

		public T Get<T>(object key)
		{
			string k = key.ToString();
			if (string.IsNullOrEmpty(k) || !this.cache.ContainsKey(k)) {
				return default;
			}
			return (T)this.cache[k];
		}

		public bool Set(object key, object item)
		{
			string k = key.ToString();
			if (string.IsNullOrEmpty(k) || item == null) {
				return false;
			}
			if (this.cache.ContainsKey(k)) {
				this.cache[k] = item;
				return true;
			}
			this.cache.Add(k, item);
			return true;
		}

		public void Delete(object key)
		{
			string k = key.ToString();
			if (this.cache.ContainsKey(k)) {
				this.cache.Remove(k);
			}
		}

		public bool IsCached(object key)
		{
			string k = key.ToString();
			if (string.IsNullOrEmpty(k)) {
				return false;
			}
			return this.cache.ContainsKey(k);
		}

		public void Clear()
		{
			this.cache.Clear();
		}
	}
}