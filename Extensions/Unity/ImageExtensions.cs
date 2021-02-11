using UnityEngine;
using UnityEngine.UI;

namespace Unitils
{
	public static class ImageExtensions
	{
		public static void SetImage(this Image self, Sprite sprite, bool isNativeSize = true)
		{
			if (self == null) return;
			self.sprite = sprite;
			if (isNativeSize) self.SetNativeSize();
		}
	}
}