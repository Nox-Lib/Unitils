using UnityEngine;

namespace Unitils
{
	public class LabelAttribute : PropertyAttribute
	{
		public readonly string label;

		public LabelAttribute(string label)
		{
			this.label = label;
		}
	}
}