using UnityEngine;

namespace Unitils
{
	public static partial class Utils
	{
		public static class Time
		{
			public static float SyncFrame(float secondsAmount)
			{
				return secondsAmount * UnityEngine.Time.deltaTime;
			}

			public static float ToAbsSin(float duration, float bottom)
			{
				float t = UnityEngine.Time.time % duration;
				t = Mathf.PI / duration * t;
				bottom = Mathf.Clamp01(bottom);
				return Mathf.Abs(Mathf.Sin(t)) * (1f - bottom) + bottom;
			}

			public static float ToAbsCos(float duration, float bottom)
			{
				float t = UnityEngine.Time.time % duration;
				t = Mathf.PI / duration * t;
				bottom = Mathf.Clamp01(bottom);
				return Mathf.Abs(Mathf.Cos(t)) * (1f - bottom) + bottom;
			}
		}
	}
}