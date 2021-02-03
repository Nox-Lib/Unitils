using System;
using System.Collections.Generic;

namespace Unitils
{
	public abstract class TableBase<TElement, TPrimaryKey>
	{
		protected abstract Func<TElement, TPrimaryKey> PrimaryKeySelector { get; }

		public RangeList<TElement> All => new RangeList<TElement>(this.source, 0, this.source.Length);

		protected TElement[] source;

		protected TableBase(TElement[] source)
		{
			this.source = source;
			this.source = this.CloneAndSortBy(this.PrimaryKeySelector, Comparer<TPrimaryKey>.Default);
		}

		protected TElement ThrowKeyNotFound<TKey>(TKey key)
		{
			throw new KeyNotFoundException($"{typeof(TElement).FullName} key: {key}");
		}

		protected TElement[] CloneAndSortBy<TKey>(Func<TElement, TKey> indexSelector, IComparer<TKey> cmparer)
		{
			TKey[] keys = new TKey[this.source.Length];
			TElement[] items = new TElement[this.source.Length];
			for (int i = 0; i < this.source.Length; i++) {
				keys[i] = indexSelector(this.source[i]);
				items[i] = this.source[i];
			}
			Array.Sort(keys, items, 0, this.source.Length, cmparer);
			return items;
		}

		protected TElement FindBy<TKey>(Func<TElement, TKey> indexSelector, IComparer<TKey> cmparer, TKey key)
		{
			int min = 0;
			int max = this.source.Length - 1;
			while (min <= max) {
				int mid = (int)(((uint)max + (uint)min) >> 1);
				TKey selected = indexSelector(this.source[mid]);
				int found = cmparer.Compare(selected, key);
				if (found == 0) return this.source[mid];
				if (found < 0) {
					min = mid + 1;
				}
				else {
					max = mid - 1;
				}
			}
			return this.ThrowKeyNotFound(key);
		}

		protected RangeList<TElement> FindMany<TKey>(TElement[] indexKeys, Func<TElement, TKey> keySelector, IComparer<TKey> comparer, TKey key)
		{
			int min = this.LowerBound(indexKeys, 0, indexKeys.Length, key, keySelector, comparer);
			if (min == -1) return RangeList<TElement>.Empty;

			int max = this.UpperBound(indexKeys, 0, indexKeys.Length, key, keySelector, comparer);
			if (max == -1) return RangeList<TElement>.Empty;

			return new RangeList<TElement>(indexKeys, min, max);
		}

		protected int LowerBound<TKey>(TElement[] array, int min, int max, TKey key, Func<TElement, TKey> selector, IComparer<TKey> comparer)
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

		protected int UpperBound<TKey>(TElement[] array, int min, int max, TKey key, Func<TElement, TKey> selector, IComparer<TKey> comparer)
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