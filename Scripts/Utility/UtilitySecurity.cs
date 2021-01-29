using System;
using System.Text;
using System.Security.Cryptography;

namespace Unitils
{
	public static partial class Utils
	{
		public static class Security
		{
			public static string ConvartToMD5Key(byte[] target)
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

			public static bool IsMatchMD5Key(string md5Key1, string md5Key2)
			{
				StringComparer stringComparer = StringComparer.OrdinalIgnoreCase;
				return stringComparer.Compare(md5Key1, md5Key2) == 0;
			}
		}
	}
}