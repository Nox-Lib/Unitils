using System.Collections;
using System.Collections.Generic;

namespace Unitils
{
	public class LatestList<T> : IEnumerable<T>
	{
		private readonly List<T> list;

		public int Size { get; }
		public int Count => this.list.Count;

		public T First => this.list.Count <= 0 ? default : this.list[0];
		public T Last => this.list.Count <= 0 ? default : this.list[this.Count - 1];

		public T this[int index] => this.list[index];

		public LatestList(int size)
		{
			this.Size = size;
			this.list = new List<T>();
		}

		public LatestList(int size, T fillValue) : this(size)
		{
			for (int i = 0; i < this.Size; i++) {
				this.Add(fillValue);
			}
		}

		public LatestList(int size, IEnumerable<T> args) : this(size)
		{
			foreach (T item in args) {
				this.Add(item);
			}
		}

		public LatestList(int size, params T[] args) : this(size)
		{
			for (int i = 0; i < args.Length; i++) {
				this.Add(args[i]);
			}
		}

		public void Add(T item)
		{
			this.list.Insert(0, item);
			if (this.list.Count > this.Size) {
				this.list.RemoveAt(this.list.Count - 1);
			}
		}

		public IEnumerator<T> GetEnumerator()
		{
			return this.list.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.list.GetEnumerator();
		}
	}
}