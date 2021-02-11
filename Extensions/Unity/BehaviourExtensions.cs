using UnityEngine;

namespace Unitils
{
	public static class BehaviourExtensions
	{
		public static void SafeEnabled(this Behaviour self, bool enabled)
		{
			if (self != null) self.enabled = enabled;
		}

		public static T GetOrAddComponent<T>(this Behaviour self) where T : Component
		{
			return self != null ? self.gameObject.GetOrAddComponent<T>() : null;
		}
	}
}