using System;

namespace Unitils
{
	public static partial class Utils
	{
		public static class Math
		{
			public static bool InRange<T>(T target, T min, T max) where T : IComparable
			{
				return target.CompareTo(min) >= 0 && target.CompareTo(max) <= 0;
			}
		}
	}
}