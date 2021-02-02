using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unitils
{
	public class RangeList<T> : IEnumerable<T>, IReadOnlyList<T>, IList<T>
	{
		public static RangeList<T> Empty => new RangeList<T>(null, -1, -1);

		private readonly T[] ordered;
		private int min;
		private int max;
		private bool hasValue;

		public int Count => this.hasValue ? this.max - this.min + 1 : 0;
		public T First => this[0];
		public T Last => this[this.Count - 1];

		public bool IsReadOnly => true;

		public T this[int index] {
			get {
				if (!this.hasValue || index < 0 || index >= this.Count) {
					throw new ArgumentOutOfRangeException();
				}
				return this.ordered[this.min + index];
			}
		}

		public RangeList(T[] ordered, int min, int max)
		{
			this.ordered = ordered;
			this.min = Mathf.Max(Mathf.Min(min, max), 0);
			this.max = Mathf.Min(Mathf.Max(min, max), this.ordered.Length - 1);
			this.hasValue = this.ordered != null && this.ordered.Length > 0;
		}

		public IEnumerator<T> GetEnumerator()
		{
			int count = this.Count;
			for (int i = 0; i < count; i++) {
				yield return this[i];
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public int IndexOf(T item)
		{
			int i = 0;
			foreach (T element in this) {
				if (EqualityComparer<T>.Default.Equals(element, item)) return i;
				i++;
			}
			return -1;
		}

		public bool Contains(T item)
		{
			int count = this.Count;
			for (int i = 0; i < count; i++) {
				T element = this[i];
				if (EqualityComparer<T>.Default.Equals(element, item)) return true;
			}
			return false;
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			int count = this.Count;
			Array.Copy(this.ordered, this.min, array, arrayIndex, count);
		}

		T IList<T>.this[int index] {
			get => this[index];
			set => throw new NotImplementedException();
		}

		public void Insert(int index, T item)
		{
			throw new NotSupportedException();
		}

		public void RemoveAt(int index)
		{
			throw new NotSupportedException();
		}

		public void Add(T item)
		{
			throw new NotSupportedException();
		}

		public void Clear()
		{
			throw new NotSupportedException();
		}

		public bool Remove(T item)
		{
			throw new NotSupportedException();
		}
	}
}