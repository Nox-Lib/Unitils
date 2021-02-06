using System;
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
				if (string.IsNullOrEmpty(separatedText)) return new int[0];
				string[] splitted = separatedText.Split(separator);
				return splitted.Select(_ => int.Parse(_)).ToArray();
			}

			public static List<int> ToIntList(string separatedText, char separator)
			{
				if (string.IsNullOrEmpty(separatedText)) return new List<int>();
				string[] splitted = separatedText.Split(separator);
				return splitted.Select(_ => int.Parse(_)).ToList();
			}

			public static string[] Split(string target, Func<int, char, bool> predicate)
			{
				List<int> indices = new List<int>();
				for (int i = 0; i < target.Length - 1; i++) {
					if (predicate(i, target[i])) indices.Add(i);
				}
				indices.Add(target.Length);
				string[] result = new string[indices.Count];
				int beforeIndex = 0;
				for (int i = 0; i < indices.Count; i++) {
					result[i] = target.Substring(beforeIndex, indices[i] - beforeIndex);
					beforeIndex = indices[i];
				}
				return result;
			}

			public static string[] SplitByUpper(string target)
			{
				return Split(target, (i, c) => char.IsUpper(c));
			}

			public static string ToUpper(string target, int index)
			{
				if (index >= target.Length || char.IsUpper(target[index])) return target;
				string l = target.Substring(0, index);
				string r = target.Substring(index + 1, target.Length - index - 1);
				return l + char.ToUpper(target[index]) + r;
			}

			public static string ToLower(string target, int index)
			{
				if (index >= target.Length || char.IsLower(target[index])) return target;
				string l = target.Substring(0, index);
				string r = target.Substring(index + 1, target.Length - index - 1);
				return l + char.ToLower(target[index]) + r;
			}
		}
	}
}