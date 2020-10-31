using UnityEngine;
using System;

namespace Unitils
{
	[Serializable]
	public struct TFloat : IComparable, IComparable<float>, IEquatable<float>, IFormattable
	{
		[SerializeField] private float value;
		public float Source {
			get { return this.value; }
			set { this.value = value; }
		}

		public TFloat(float value) => this.value = value;

		public static implicit operator float(TFloat _) => _.value * Time.deltaTime;
		public static implicit operator TFloat(float _) => new TFloat(_);

		public override string ToString() => this.Source.ToString();
		public override int GetHashCode() => this.Source.GetHashCode();

		public override bool Equals(object obj)
		{
			if (obj == null || !(obj is float)) return false;
			return this.Equals((float)obj);
		}

		public int CompareTo(object obj) => this.value.CompareTo(obj);
		public int CompareTo(float other) => this.value.CompareTo(other);
		public bool Equals(float other) => this.value.Equals(other);
		public string ToString(string format, IFormatProvider formatProvider) => this.value.ToString(format, formatProvider);
	}
}