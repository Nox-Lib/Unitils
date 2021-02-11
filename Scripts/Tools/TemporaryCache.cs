using System.Collections.Generic;

namespace Unitils
{
	public class TemporaryCache
	{
		private readonly LinkedList<KeyValuePair<string, object>> cache = new LinkedList<KeyValuePair<string, object>>();

		private readonly int size;
		private readonly bool isOneTime;

		private bool IsLimit => this.size > 0 && this.cache.Count >= this.size;

		public TemporaryCache()
		{
			this.size = -1;
			this.isOneTime = false;
		}

		public TemporaryCache(int size)
		{
			this.size = size;
			this.isOneTime = false;
		}

		public TemporaryCache(bool isOneTime)
		{
			this.size = -1;
			this.isOneTime = isOneTime;
		}

		public TemporaryCache(int size, bool isOneTime)
		{
			this.size = size;
			this.isOneTime = isOneTime;
		}

		public T Get<T>(object key)
		{
			LinkedListNode<KeyValuePair<string, object>> node = this.GetNode(key);
			if (node == null) return default;

			object result = node.Value.Value;
			if (this.isOneTime) {
				this.cache.Remove(node);
			}
			return (T)result;
		}

		public bool Set(object key, object item)
		{
			string k = key.ToString();
			if (string.IsNullOrEmpty(k) || item == null) {
				return false;
			}
			this.Remove(k);
			this.cache.AddLast(new KeyValuePair<string, object>(k, item));

			if (this.IsLimit) {
				this.cache.RemoveFirst();
			}
			return true;
		}

		public bool IsCached(object key)
		{
			return this.GetNode(key) != null;
		}

		public void Clear()
		{
			this.cache.Clear();
		}


		private LinkedListNode<KeyValuePair<string, object>> GetNode(object key)
		{
			string k = key.ToString();
			if (string.IsNullOrEmpty(k)) return null;

			LinkedListNode<KeyValuePair<string, object>> node = this.cache.First;
			while (node != null) {
				if (node.Value.Key.Equals(k)) break;
				node = node.Next;
			}
			return node;
		}

		private void Remove(object key)
		{
			LinkedListNode<KeyValuePair<string, object>> node = this.GetNode(key);
			if (node != null) {
				this.cache.Remove(node);
			}
		}
	}
}