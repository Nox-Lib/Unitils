using System;
using System.Collections.Generic;

namespace Unitils
{
	public abstract class ReadonlyTable<TElement>
	{
		public RangeList<TElement> All => new RangeList<TElement>(this.source, 0, this.source.Length);
		public int Count => this.source.Length;

		protected TElement[] source;

		protected ReadonlyTable(TElement[] ordered)
		{
			this.source = ordered;
		}

		protected TElement ThrowKeyNotFound<TKey>(TKey key)
		{
			throw new KeyNotFoundException($"{typeof(TElement).FullName} key: {key}");
		}

		protected TElement[] CloneAndSortBy<TKey>(Func<TElement, TKey> indexSelector, IComparer<TKey> comparer)
		{
			TKey[] keys = new TKey[this.source.Length];
			TElement[] items = new TElement[this.source.Length];
			for (int i = 0; i < this.source.Length; i++) {
				keys[i] = indexSelector(this.source[i]);
				items[i] = this.source[i];
			}
			Array.Sort(keys, items, 0, this.source.Length, comparer);
			return items;
		}

		protected TElement FindUnique<TKey>(TElement[] indexArray, Func<TElement, TKey> keySelector, IComparer<TKey> comparer, TKey key)
		{
			int min = 0;
			int max = indexArray.Length - 1;
			while (min <= max) {
				int mid = (int)(((uint)max + (uint)min) >> 1);
				TKey selected = keySelector(this.source[mid]);
				int found = comparer.Compare(selected, key);
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

		protected RangeList<TElement> FindMany<TKey>(TElement[] indexArray, Func<TElement, TKey> keySelector, IComparer<TKey> comparer, TKey key)
		{
			int minIndex = this.LowerBound(indexArray, 0, indexArray.Length, key, keySelector, comparer);
			if (minIndex == -1) return RangeList<TElement>.Empty;

			int maxIndex = this.UpperBound(indexArray, 0, indexArray.Length, key, keySelector, comparer);
			if (maxIndex == -1) return RangeList<TElement>.Empty;

			return new RangeList<TElement>(indexArray, minIndex, maxIndex);
		}


		private int LowerBound<TKey>(TElement[] array, int min, int max, TKey key, Func<TElement, TKey> selector, IComparer<TKey> comparer)
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

		private int UpperBound<TKey>(TElement[] array, int min, int max, TKey key, Func<TElement, TKey> selector, IComparer<TKey> comparer)
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