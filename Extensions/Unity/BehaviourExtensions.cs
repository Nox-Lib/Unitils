using UnityEngine;

namespace Unitils
{
	public static class BehaviourExtensions
	{
		public static void SafeEnabled(this Behaviour self, bool enabled)
		{
			if (self != null) self.enabled = enabled;
		}
	}
}