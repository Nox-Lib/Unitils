using UnityEngine;

namespace Unitils
{
	public static class GameObjectExtensions
	{
		public static void SafeActive(this GameObject self, bool isActive)
		{
			if (self != null) self.SetActive(isActive);
		}

		public static T GetOrAddComponent<T>(this GameObject self) where T : Component
		{
			if (self == null) return null;

			T component = self.GetComponent<T>();
			if (component == null) {
				component = self.AddComponent<T>();
			}
			return component;
		}
	}
}