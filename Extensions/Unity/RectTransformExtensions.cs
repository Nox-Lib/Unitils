using UnityEngine;

namespace Unitils
{
	public static class RectTransformExtensions
	{
		#region AnchoredPosition

		public static void SetAnchoredPosition(this RectTransform self, float x, float y)
		{
			self.anchoredPosition = new Vector2(x, y);
		}

		public static void SetAnchoredPositionX(this RectTransform self, float x)
		{
			self.anchoredPosition = new Vector2(x, self.anchoredPosition.y);
		}

		public static void SetAnchoredPositionY(this RectTransform self, float y)
		{
			self.anchoredPosition = new Vector2(self.anchoredPosition.x, y);
		}
		#endregion


		#region SizeDelta

		public static void SetSizeDelta(this RectTransform self, float x, float y)
		{
			self.sizeDelta = new Vector2(x, y);
		}

		public static void SetSizeDeltaX(this RectTransform self, float x)
		{
			self.sizeDelta = new Vector2(x, self.sizeDelta.y);
		}

		public static void SetSizeDeltaY(this RectTransform self, float y)
		{
			self.sizeDelta = new Vector2(self.sizeDelta.x, y);
		}
		#endregion
	}
}