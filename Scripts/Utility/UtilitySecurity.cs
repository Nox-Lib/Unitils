using System;
using System.Text;
using System.Security.Cryptography;

namespace Unitils
{
	public static partial class Utils
	{
		public static class Security
		{
			public static string GetMD5(string target)
			{
				return GetMD5(Encoding.UTF8.GetBytes(target));
			}

			public static string GetMD5(byte[] target)
			{
				if (target == null) return string.Empty;

				MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();

				byte[] md5Hash = md5.ComputeHash(target);
				md5.Clear();

				StringBuilder stringBuilder = new StringBuilder();
				foreach (byte b in md5Hash) {
					stringBuilder.Append(b.ToString("x2"));
				}
				return stringBuilder.ToString();
			}

			public static bool IsMatchMD5(string a, string b)
			{
				StringComparer stringComparer = StringComparer.OrdinalIgnoreCase;
				return stringComparer.Compare(a, b) == 0;
			}
		}
	}
}