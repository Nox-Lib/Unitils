using System;
using System.Collections.Generic;

namespace Unitils
{
	public abstract class ReadonlyTable<TElement, TPrimaryKey>
	{
		protected abstract Func<TElement, TPrimaryKey> PrimaryKeySelector { get; }

		public RangeList<TElement> All => new RangeList<TElement>(this.source, 0, this.source.Length);

		protected TElement[] source;

		protected ReadonlyTable(TElement[] source)
		{
			this.source = source;
			this.source = this.CloneAndSortBy(this.PrimaryKeySelector, Comparer<TPrimaryKey>.Default);
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

		protected RangeList<TElement> FindUniqueRange<TKey>(TElement[] indexArray, Func<TElement, TKey> keySelector, IComparer<TKey> comparer, TKey min, TKey max)
		{
			int minIndex = FindClosest(indexArray, 0, indexArray.Length, min, keySelector, comparer, false);
			int maxIndex = FindClosest(indexArray, 0, indexArray.Length, max, keySelector, comparer, true);

			if (minIndex == -1) minIndex = 0;
			if (maxIndex == indexArray.Length) maxIndex -= 1;

			return new RangeList<TElement>(indexArray, minIndex, maxIndex);
		}

		protected RangeList<TElement> FindMany<TKey>(TElement[] indexArray, Func<TElement, TKey> keySelector, IComparer<TKey> comparer, TKey key)
		{
			int minIndex = this.LowerBound(indexArray, 0, indexArray.Length, key, keySelector, comparer);
			if (minIndex == -1) return RangeList<TElement>.Empty;

			int maxIndex = this.UpperBound(indexArray, 0, indexArray.Length, key, keySelector, comparer);
			if (maxIndex == -1) return RangeList<TElement>.Empty;

			return new RangeList<TElement>(indexArray, minIndex, maxIndex);
		}


		private static int FindClosest<TKey>(TElement[] array, int min, int max, TKey key, Func<TElement, TKey> selector, IComparer<TKey> comparer, bool selectLower)
		{
			if (array.Length == 0) return -1;

			min -= 1;

			while (max - min > 1) {
				var mid = min + ((max - min) >> 1);
				var found = comparer.Compare(selector(array[mid]), key);

				if (found == 0) {
					min = max = mid;
					break;
				}
				if (found >= 1) {
					max = mid;
				}
				else {
					min = mid;
				}
			}

			return selectLower ? min : max;
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