using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unitils
{
	public abstract class WritableTable<TElement, TPrimaryKey> : TableBase<TElement, TPrimaryKey>
	{
		public WritableTable(TElement[] source) : base(source) {}

		public string Serialize()
		{
			return JsonUtility.ToJson(new JsonSerializableArray<TElement>(this.source));
		}

		public void Add(TElement item)
		{
			int length = this.source.Length + 1;
			TPrimaryKey[] keys = new TPrimaryKey[length];
			TElement[] items = new TElement[length];
			for (int i = 0; i < this.source.Length; i++) {
				keys[i] = this.PrimaryKeySelector(this.source[i]);
				items[i] = this.source[i];
			}
			keys[length - 1] = this.PrimaryKeySelector(item);
			items[length - 1] = item;
			Array.Sort(keys, items, 0, length, Comparer<TPrimaryKey>.Default);
			this.source = items;
		}

		public void RemoveAt(int index)
		{
			TElement[] newArray = new TElement[this.source.Length - 1];
			Array.Copy(this.source, 0, newArray, 0, index);
			Array.Copy(this.source, index + 1, newArray, index, this.source.Length - index - 1);
			this.source = newArray;
		}
	}
}