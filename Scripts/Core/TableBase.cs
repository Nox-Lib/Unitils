using System;
using System.Collections.Generic;

namespace Unitils
{
	public abstract class TableBase<T>
	{
		protected T[] source;
		public RangeList<T> All => new RangeList<T>(this.source, 0, this.source.Length);

		protected TableBase(T[] source)
		{
			this.source = source;
		}

		protected T ThrowKeyNotFound<TKey>(TKey key)
		{
			throw new KeyNotFoundException($"{typeof(T).FullName} key: {key}");
		}

		protected T[] CloneAndSortBy<TKey>(Func<T, TKey> indexSelector, IComparer<TKey> cmparer)
		{
			TKey[] keys = new TKey[this.source.Length];
			T[] items = new T[this.source.Length];
			for (int i = 0; i < this.source.Length; i++) {
				keys[i] = indexSelector(this.source[i]);
				items[i] = this.source[i];
			}
			Array.Sort(keys, items, 0, this.source.Length, cmparer);
			return items;
		}

		protected T FindBy<TKey>(Func<T, TKey> indexSelector, IComparer<TKey> cmparer, TKey key)
		{
			int min = 0;
			int max = this.source.Length - 1;
			while (min <= max) {
				int mid = (int)(((uint)max + (uint)min) >> 1);
				TKey selected = indexSelector(this.source[mid]);
				int found = cmparer.Compare(selected, key);
				if (found == 0) { return this.source[mid]; }
				if (found < 0) { min = mid + 1; }
				else { max = mid - 1; }
			}
			return this.ThrowKeyNotFound(key);
		}

		protected RangeList<T> FindMany<TKey>(T[] indexKeys, Func<T, TKey> keySelector, IComparer<TKey> comparer, TKey key)
		{
			int min = this.LowerBound(indexKeys, 0, indexKeys.Length, key, keySelector, comparer);
			if (min == -1) return RangeList<T>.Empty;

			int max = this.UpperBound(indexKeys, 0, indexKeys.Length, key, keySelector, comparer);
			if (max == -1) return RangeList<T>.Empty;

			return new RangeList<T>(indexKeys, min, max);
		}

		private int LowerBound<TKey>(T[] array, int min, int max, TKey key, Func<T, TKey> selector, IComparer<TKey> comparer)
		{
			while (min < max) {
				int mid = min + ((max - min) >> 1);
				int found = comparer.Compare(key, selector(array[mid]));

				if (found <= 0) {
					max = mid;
				}
				else {
					min = mid + 1;
				}
			}

			int index = min;
			if (index == -1 || array.Length <= index) return -1;

			return (comparer.Compare(key, selector(array[index])) == 0) ? index : -1;
		}

		private int UpperBound<TKey>(T[] array, int min, int max, TKey key, Func<T, TKey> selector, IComparer<TKey> comparer)
		{
			while (min < max) {
				int mid = min + ((max - min) >> 1);
				int found = comparer.Compare(key, selector(array[mid]));

				if (found >= 0) {
					min = mid + 1;
				}
				else {
					max = mid;
				}
			}

			int index = (min == 0) ? 0 : min - 1;
			if (index == -1 || array.Length <= index) return -1;

			return (comparer.Compare(key, selector(array[index])) == 0) ? index : -1;
		}
	}
}