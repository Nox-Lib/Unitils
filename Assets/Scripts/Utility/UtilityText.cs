using System.Collections.Generic;
using System.Linq;

namespace Unitils
{
	public static partial class Utils
	{
		public static class Text
		{
			public static string ToSeparatedText<T>(IEnumerable<T> targets, char separator)
			{
				string result = string.Empty;
				if (targets == null) return result;

				foreach (T target in targets) {
					result += target.ToString() + separator;
				}
				if (targets.Any()) {
					result = result.TrimEnd(separator);
				}
				return result;
			}

			public static int[] ToIntArray(string separatedText, char separator)
			{
				if (string.IsNullOrEmpty(separatedText)) {
					return new int[0];
				}
				string[] splitted = separatedText.Split(separator);
				return splitted.Select(x => int.Parse(x)).ToArray();
			}

			public static List<int> ToIntList(string separatedText, char separator)
			{
				if (string.IsNullOrEmpty(separatedText)) {
					return new List<int>();
				}
				string[] splitted = separatedText.Split(separator);
				return splitted.Select(x => int.Parse(x)).ToList();
			}
		}
	}
}