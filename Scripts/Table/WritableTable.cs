using System;
using System.Collections.Generic;
using System.Linq;

namespace Unitils
{
	public abstract class WritableTable<TElement>
	{
		public IReadOnlyList<TElement> All => this.source;
		public int Count => this.source.Count;

		protected List<TElement> source;

		protected WritableTable(List<TElement> ordered = null)
		{
			this.source = ordered ?? new List<TElement>();
		}

		protected List<TElement> CloneAndSortBy<TKey>(Func<TElement, TKey> indexSelector, IComparer<TKey> comparer)
		{
			List<TElement> result = new List<TElement>(this.source);
			result.Sort((a, b) => comparer.Compare(indexSelector(a), indexSelector(b)));
			return result;
		}

		protected void Insert<TKey>(TElement element, List<TElement> indexList, Func<TElement, TKey> keySelector, IComparer<TKey> comparer, bool isUnique)
		{
			TKey key = keySelector(element);
			if (isUnique && this.FindUnique(indexList, keySelector, comparer, key, false) != null) {
				throw new ArgumentException($"key has already been added.Key: {key}");
			}
			int insertIndex = 0;
			for (; insertIndex < indexList.Count; insertIndex++) {
				if (comparer.Compare(keySelector(indexList[insertIndex]), key) > 0) break;
			}
			indexList.Insert(insertIndex, element);
		}

		protected TElement ThrowKeyNotFound<TKey>(TKey key)
		{
			throw new KeyNotFoundException($"{typeof(TElement).FullName} key: {key}");
		}

		protected TElement FindUnique<TKey>(List<TElement> indexList, Func<TElement, TKey> keySelector, IComparer<TKey> comparer, TKey key, bool assertion = true)
		{
			int min = 0;
			int max = indexList.Count - 1;
			while (min <= max) {
				int mid = (int)(((uint)max + (uint)min) >> 1);
				TKey selected = keySelector(indexList[mid]);
				int found = comparer.Compare(selected, key);
				if (found == 0) return indexList[mid];
				if (found < 0) {
					min = mid + 1;
				}
				else {
					max = mid - 1;
				}
			}
			return assertion ? this.ThrowKeyNotFound(key) : default;
		}

		protected IEnumerable<TElement> FindMany<TKey>(List<TElement> indexList, Func<TElement, TKey> keySelector, IComparer<TKey> comparer, TKey key)
		{
			return indexList.Where(_ => comparer.Compare(keySelector(_), key) == 0);
		}
	}
}