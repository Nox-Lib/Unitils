using UnityEngine;

namespace Unitils
{
	public class EnumFilterAttribute : PropertyAttribute
	{
		public readonly bool isIgnoreMode;
		public readonly string[] items;

		public EnumFilterAttribute(bool isIgnoreMode, params string[] items)
		{
			this.isIgnoreMode = isIgnoreMode;
			this.items = items;
		}
	}
}