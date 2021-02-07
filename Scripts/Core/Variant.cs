using System;
using System.Collections;
using System.Collections.Generic;

namespace Unitils
{
    public class Variant : IEnumerable
	{
		public static Variant LIST => new Variant(new List<object>());
		public static Variant HASH => new Variant(new Dictionary<string, object>());

		public static Variant EMPTY => new Variant(null);

		private readonly object data;
		private readonly bool isList;
		private readonly bool isHash;

		public Variant this[object id] {
			get {
				if (this.isList) {
					int.TryParse(id.ToString(), out int index);
					if (Utils.Math.InRange(index, 0, this.Count - 1)) {
						return new Variant((this.data as List<object>)[index]);
					}
				}
				if (this.isHash) {
					string key = id.ToString();
					if (this.ContainsKey(key)) {
						return new Variant((this.data as Dictionary<string, object>)[key]);
					}
				}
				return EMPTY;
			}
			set {
				if (this.isList) {
					int.TryParse(id.ToString(), out int index);
					object item = value;
					if (item is Variant) {
						item = (item as Variant).data;
					}
					if (!this.Contains(item)) {
						(this.data as List<object>).Add(item);
					}
					else {
						(this.data as List<object>)[index] = item;
					}
				}
				if (this.isHash) {
					string k = id.ToString();
					object item = value;
					if (item is Variant) {
						item = (item as Variant).data;
					}
					if (!this.ContainsKey(k)) {
						(this.data as Dictionary<string, object>).Add(k, item);
					}
					else {
						(this.data as Dictionary<string, object>)[k] = item;
					}
				}
			}
		}

		public bool IsEmpty => this.data == null;

		public int Count {
			get {
				if (this.isList) return (this.data as List<object>).Count;
				if (this.isHash) return (this.data as Dictionary<string, object>).Count;
				return this.IsEmpty ? 0 : 1;
			}
		}

		public int Int => int.TryParse(this.data.ToString(), out int result) ? result : 0;
		public long Long => long.TryParse(this.data.ToString(), out long result) ? result : 0;
		public float Float => float.TryParse(this.data.ToString(), out float result) ? result : 0f;
		public double Double => double.TryParse(this.data.ToString(), out double result) ? result : 0f;
		public string String => this.data.ToString();
		public bool Bool => this.Int.Equals(1) || this.String.ToLower().Equals("true");

		public DateTime DateTime => this.ToDateTime();
		private DateTime ToDateTime() => DateTime.TryParse(this.data.ToString(), out DateTime result) ? result : DateTime.MinValue;


		public Variant(object data)
		{
			this.data = data;
			this.isList = this.data is List<object>;
			this.isHash = this.data is Dictionary<string, object>;
		}


		public bool Contains(object item)
		{
			return this.isList && (this.data as List<object>).Contains(item);
		}

		public bool ContainsKey(object key)
		{
			string k = key.ToString();
			return this.isHash && !string.IsNullOrEmpty(k) && (this.data as Dictionary<string, object>).ContainsKey(k);
		}

		public void Add(object item)
		{
			if (item == null || !this.isList || this.Contains(item)) return;
			if (item is Variant) {
				item = (item as Variant).data;
			}
			(this.data as List<object>).Add(item);
		}

		public void Add(object key, object item)
		{
			if (item == null || !this.isHash) return;
			string k = key.ToString();
			if (this.ContainsKey(k)) return;	
			if (item is Variant) {
				item = (item as Variant).data;
			}
			(this.data as Dictionary<string, object>).Add(k, item);
		}


		public override string ToString()
		{
			return this.data == null ? "data is null." : this.String;
		}


		public static implicit operator int(Variant value) => value.Int;
		public static implicit operator long(Variant value) => value.Long;
		public static implicit operator float(Variant value) => value.Float;
		public static implicit operator double(Variant value) => value.Double;
		public static implicit operator string(Variant value) => value.String;
		public static implicit operator bool(Variant value) => value.Bool;
		public static implicit operator DateTime(Variant value) => value.DateTime;

		public static implicit operator Variant(int value) => new Variant(value);
		public static implicit operator Variant(long value) => new Variant(value);
		public static implicit operator Variant(float value) => new Variant(value);
		public static implicit operator Variant(double value) => new Variant(value);
		public static implicit operator Variant(string value) => new Variant(value);
		public static implicit operator Variant(bool value) => new Variant(value);


		public IEnumerator GetEnumerator()
		{
			return new VariantEnumerator(this);
		}

		private class VariantEnumerator : IEnumerator
		{
			private readonly Variant root;
			private readonly List<string> keys;
			private int cursor;
			private object current;

			public VariantEnumerator(Variant root)
			{
				this.root = root;
				this.current = null;

				this.cursor = 0;
				if (this.root.isHash) {
					this.keys = new List<string>((this.root.data as Dictionary<string, object>).Keys);
				}
			}

			public object Current => this.current;

			public bool MoveNext()
			{
				if (this.root.isList && this.cursor < this.root.Count) {
					this.current = this.root[this.cursor++];
					return true;
				}
				if (this.root.isHash && this.cursor < this.root.Count) {
					string key = this.keys[this.cursor++];
					this.current = new KeyValuePair<string, Variant>(key, this.root[key]);
					return true;
				}
				return false;
			}

			public void Reset()
			{
				this.current = null;
				this.cursor = 0;
			}
		}
	}
}