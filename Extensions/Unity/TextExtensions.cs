using UnityEngine.UI;

namespace Unitils
{
	public static class TextExtensions
	{
		public static void SetText(this Text self, object text)
		{
			if (self != null) self.text = text.ToString();
		}
	}
}