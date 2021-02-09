using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unitils
{
	public abstract class WritableTable<TElement, TPrimaryKey> : TableBase<TElement, TPrimaryKey>
	{
		private List<TElement> list = null;

		public WritableTable(TElement[] source) : base(source) {}

		public string Serialize()
		{
			return JsonUtility.ToJson(new JsonSerializableArray<TElement>(this.source));
		}

		public void Add(TElement item)
		{
			if (this.Exists(this.PrimaryKeySelector, Comparer<TPrimaryKey>.Default, item)) {
				throw new ArgumentException($"already exists primary key: {this.PrimaryKeySelector(item)}");
			}
			if (this.list == null) this.list = new List<TElement>(this.source);

			this.list.Add(item);

			Comparer<TPrimaryKey> comparer = Comparer<TPrimaryKey>.Default;
			this.list.Sort((a, b) => comparer.Compare(this.PrimaryKeySelector(a), this.PrimaryKeySelector(b)));

			this.source = this.list.ToArray();
		}

		public void RemoveAt(int index)
		{
			if (this.list == null) this.list = new List<TElement>(this.source);
			this.list.RemoveAt(index);
			this.source = this.list.ToArray();
		}
	}
}