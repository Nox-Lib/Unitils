using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine;

namespace Unitils
{
	public abstract class Enumeration<T> : Enumeration
	{
		public T Contents { get; }

		protected Enumeration(int id, string name, T contents) : base(id, name)
		{
			this.Contents = contents;
		}
	}

	public abstract class Enumeration : IComparable
	{
		[SerializeField] private int id;
		[SerializeField] private string name;

		public int Id {
			get { return this.id; }
			set { this.id = value; }
		}

		public string Name {
			get { return this.name; }
			set { this.name = value; }
		}

		protected Enumeration(int id, string name)
		{
			this.Id = id;
			this.Name = name;
		}

		public override string ToString() => this.Name;
		public override int GetHashCode() => this.Id.GetHashCode();

		public static IEnumerable<T> GetAll<T>() where T : Enumeration
		{
			FieldInfo[] filedInfos = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
			return filedInfos.Select(_ => _.GetValue(null)).Cast<T>();
		}

		public static IEnumerable<Enumeration> GetAll(Type type)
		{
			FieldInfo[] filedInfos = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
			return filedInfos.Select(_ => _.GetValue(null)).Cast<Enumeration>();
		}

		public override bool Equals(object obj)
		{
			if (!(obj is Enumeration other)) return false;
			bool isTypeMatches = this.GetType().Equals(obj.GetType());
			bool isValueMatches = this.Id.Equals(other.Id);
			return isTypeMatches && isValueMatches;
		}

		public int CompareTo(object other) => this.Id.CompareTo(((Enumeration)other).Id);
	}
}