using System.Collections.Generic;
using UnityEngine;

namespace Unitils
{
	public class JsonSerializableList<T>
	{
		[SerializeField] private List<T> list;
		public List<T> Data => this.list;

		public JsonSerializableList(List<T> list)
		{
			this.list = list;
		}
	}
}