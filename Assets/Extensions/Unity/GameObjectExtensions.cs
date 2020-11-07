using UnityEngine;

namespace Unitils
{
	public static class GameObjectExtensions
	{
		public static void SafeActive(this GameObject self, bool isActive)
		{
			if (self != null) self.SetActive(isActive);
		}
	}
}