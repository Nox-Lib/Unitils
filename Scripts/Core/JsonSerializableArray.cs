using UnityEngine;

namespace Unitils
{
	public class JsonSerializableArray<T>
	{
		[SerializeField] private T[] list;
		public T[] Data => this.list;

		public JsonSerializableArray(T[] array)
		{
			this.list = array;
		}
	}
}