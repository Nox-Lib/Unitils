using System.Collections.Generic;

namespace Unitils
{
	public static partial class Utils
	{
		private static readonly System.Random random = new System.Random();

		public static T[] ArrayShuffle<T>(T[] array)
		{
			int n = array.Length;
			while (n > 1) {
				n--;
				int k = random.Next(n + 1);
				T tmp = array[k];
				array[k] = array[n];
				array[n] = tmp;
			}
			return array;
		}

		public static List<T> ListShuffle<T>(List<T> list)
		{
			int n = list.Count;
			while (n > 1) {
				n--;
				int k = random.Next(n + 1);
				T tmp = list[k];
				list[k] = list[n];
				list[n] = tmp;
			}
			return list;
		}
	}
}