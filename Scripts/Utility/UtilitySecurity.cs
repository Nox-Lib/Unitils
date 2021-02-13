using System;
using System.Text;
using System.Security.Cryptography;

namespace Unitils
{
	public static partial class Utils
	{
		public static class Security
		{
			private static readonly Random random = new Random();

			public static string GeneratePassword(int length, bool useNumber = true, bool useUpper = true, bool useLower = true, bool useSymbol = true)
			{
				if (length <= 0) return "";

				const string NUMBERS = "0123456789";
				const string UPPER_ALPHABETS = "abcdefghijklmnopqrstuvwxyz";
				const string LOWER_ALPHABETS = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
				const string SYMBOLS = "!@#$%&*()_+-=|:;<>?";

				string source = "";
				if (useNumber) source += NUMBERS;
				if (useUpper) source += UPPER_ALPHABETS;
				if (useLower) source += LOWER_ALPHABETS;
				if (useSymbol) source += SYMBOLS;

				if (source.Length <= 0) return "";

				StringBuilder builder = new StringBuilder(length);
				for (int i = 0; i < length; i++) {
					builder.Append(source[random.Next(source.Length)]);
				}

				return builder.ToString();
			}


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


			public static byte[] EncryptAES(string text, string key, string iv)
			{
				AesCryptoServiceProvider aesCryptoService = new AesCryptoServiceProvider
				{
					Key = Encoding.UTF8.GetBytes(key),
					IV = Encoding.UTF8.GetBytes(iv)
				};
				ICryptoTransform encryptor = aesCryptoService.CreateEncryptor();
				byte[] bytes = Encoding.Unicode.GetBytes(text);

				return encryptor.TransformFinalBlock(bytes, 0, bytes.Length);
			}

			public static string DecryptAES(byte[] cipherText, string key, string iv)
			{
				AesCryptoServiceProvider aesCryptoService = new AesCryptoServiceProvider
				{
					Key = Encoding.UTF8.GetBytes(key),
					IV = Encoding.UTF8.GetBytes(iv)
				};
				ICryptoTransform decryptor = aesCryptoService.CreateDecryptor();
				byte[] bytes = decryptor.TransformFinalBlock(cipherText, 0, cipherText.Length);

				return Encoding.Unicode.GetString(bytes);
			}
		}
	}
}