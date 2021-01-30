using System.Collections.Generic;

namespace Unitils
{
	public class TemporaryCache<T> where T : class
	{
		private readonly LinkedList<KeyValuePair<string, T>> cache = new LinkedList<KeyValuePair<string, T>>();

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

		public T Get(object key)
		{
			LinkedListNode<KeyValuePair<string, T>> node = this.GetNode(key);
			if (node == null) return null;

			T result = node.Value.Value;
			if (this.isOneTime) {
				this.cache.Remove(node);
			}
			return result;
		}

		public bool Set(object key, T item)
		{
			string k = key.ToString();
			if (string.IsNullOrEmpty(k) || item == null) {
				return false;
			}
			this.Remove(k);
			this.cache.AddLast(new KeyValuePair<string, T>(k, item));

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


		private LinkedListNode<KeyValuePair<string, T>> GetNode(object key)
		{
			string k = key.ToString();
			if (string.IsNullOrEmpty(k)) return null;

			LinkedListNode<KeyValuePair<string, T>> node = this.cache.First;
			while (node != null) {
				if (node.Value.Key.Equals(k)) break;
				node = node.Next;
			}
			return node;
		}

		private void Remove(object key)
		{
			LinkedListNode<KeyValuePair<string, T>> node = this.GetNode(key);
			if (node != null) {
				this.cache.Remove(node);
			}
		}
	}
}