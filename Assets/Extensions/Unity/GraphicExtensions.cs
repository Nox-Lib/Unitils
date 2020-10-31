using UnityEngine;
using UnityEngine.UI;

namespace Unitils
{
	public static class GraphicExtensions
	{
		public static void SetAlpha(this Graphic self, float alpha)
		{
			if (self == null) return;
			Color color = self.color;
			color.a = alpha;
			self.SetColor(color);
		}

		public static void SetColor(this Graphic self, Color color)
		{
			if (self != null) self.color = color;
		}
	}
}