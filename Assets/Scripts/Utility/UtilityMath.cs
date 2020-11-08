using UnityEngine;
using System;

namespace Unitils
{
	public static partial class Utils
	{
		public static class Math
		{
			public static bool InRange<T>(T target, T min, T max) where T : IComparable
			{
				return target.CompareTo(min) >= 0 && target.CompareTo(max) <= 0;
			}

			public static float ToRoundUp(float value, int digit)
			{
				double dCoef = System.Math.Pow(10, digit);
				return (float)(value > 0 ? System.Math.Ceiling(value * dCoef) / dCoef : System.Math.Floor(value * dCoef) / dCoef);
			}

			public static float ToRoundDown(float value, int digits)
			{
				double dCoef = System.Math.Pow(10, digits);
				return (float)(value > 0 ? System.Math.Floor(value * dCoef) / dCoef : System.Math.Ceiling(value * dCoef) / dCoef);
			}

			public static float ToDegree(float radian)
			{
				return radian / Mathf.PI * 180f;
			}

			public static float ToRadian(float degree)
			{
				return Mathf.PI / 180f * degree;
			}

			public static float LinearPoint(float start, float end, float t)
			{
				return ((1f - t) * start) + (t * end);
			}

			public static float SplinePoint(float start, float end, float t, float control)
			{
				return (Mathf.Pow(1f - t, 2f) * start) + (2f * t * (1f - t) * control) + (Mathf.Pow(t, 2f) * end);
			}
		}
	}
}