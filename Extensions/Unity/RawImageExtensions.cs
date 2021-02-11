using UnityEngine;
using UnityEngine.UI;

namespace Unitils
{
	public static class RawImageExtensions
	{
		public static void SetTexture(this RawImage self, Texture texture, bool isNativeSize = true)
		{
			if (self == null) return;
			self.texture = texture;
			if (isNativeSize) self.SetNativeSize();
		}
	}
}