using UnityEngine;

namespace Unitils
{
	public static class ComponentExtensions
	{
		public static void SafeActive(this Component self, bool isActive)
		{
			if (self != null) self.gameObject.SafeActive(isActive);
		}
	}
}