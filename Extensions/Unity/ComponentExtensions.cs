using UnityEngine;

namespace Unitils
{
	public static class ComponentExtensions
	{
		public static void SafeActive(this Component self, bool isActive)
		{
			if (self != null) self.gameObject.SafeActive(isActive);
		}

		public static void DestoryGameObject(this Component self)
		{
			if (self != null) self.gameObject.SafeDestory();
		}

		public static void DestoryGameObject(this Component self, float delay)
		{
			if (self != null) self.gameObject.SafeDestory(delay);
		}
	}
}