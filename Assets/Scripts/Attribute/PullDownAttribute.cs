using UnityEngine;

namespace Unitils
{
	public class PullDownAttribute : PropertyAttribute
	{
		#region int

		public readonly bool isInt = false;
		public readonly int[] intArray;

		public PullDownAttribute(params int[] values)
		{
			this.intArray = values;
			this.isInt = true;
		}
		#endregion


		#region float

		public readonly bool isFloat = false;
		public readonly float[] floatArray;

		public PullDownAttribute(params float[] values)
		{
			this.floatArray = values;
			this.isFloat = true;
		}
		#endregion


		#region string

		public readonly bool isString = false;
		public readonly string[] stringArray;

		public PullDownAttribute(params string[] values)
		{
			this.stringArray = values;
			this.isString = true;
		}
		#endregion
	}
}