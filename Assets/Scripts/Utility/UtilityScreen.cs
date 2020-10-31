using UnityEngine;

namespace Unitils
{
	public static partial class Utils
	{
		public static class Screen
		{
			public static Vector2 GetReferenceResolution()
			{
				SystemData systemData = SystemData.Instance;

				if (systemData.ScreenMode == Define.ScreenMode.Fixed) {
					return systemData.BaseScreenSize;
				}

				Vector2 minScreenSize = systemData.MinScreenSize;
				Vector2 maxScreenSize = systemData.MaxScreenSize;

				if (minScreenSize == maxScreenSize) {
					return minScreenSize;
				}

				Vector2 screenSize = new Vector2(UnityEngine.Screen.width, UnityEngine.Screen.height);
				float screenAspect = screenSize.x / screenSize.y;

				if (screenAspect < minScreenSize.x / maxScreenSize.y) {
					return new Vector2(minScreenSize.x, maxScreenSize.y);
				}
				if (screenAspect > maxScreenSize.x / minScreenSize.y) {
					return new Vector2(maxScreenSize.x, minScreenSize.y);
				}

				screenSize = systemData.BaseScreenSize;
				bool isLandscape = screenSize.x > screenSize.y;
				float baseAspect = screenSize.x / screenSize.y;
				float aspectRatio = screenAspect / baseAspect;

				if (isLandscape) {
					if (aspectRatio > 1f) {
						screenSize.x *= aspectRatio;
					}
					else {
						screenSize.y /= aspectRatio;
					}
				}
				else {
					if (aspectRatio < 1f) {
						screenSize.y /= aspectRatio;
					}
					else {
						screenSize.x *= aspectRatio;
					}
				}

				return screenSize;
			}


			public static Vector2 GetCanvasSize()
			{
				Vector2 screenSize = new Vector2(UnityEngine.Screen.width, UnityEngine.Screen.height);
				Vector2 resolution = GetReferenceResolution();
				float scaleFactor = Mathf.Min(screenSize.x / resolution.x, screenSize.y / resolution.y);
				return screenSize / scaleFactor;
			}


			public static Vector2 GetMargin()
			{
				Vector2 resolution = GetReferenceResolution();
				Vector2 canvasSize = GetCanvasSize();

				Vector2 result;
				result.x = Mathf.Max(0f, Mathf.Abs(resolution.x - canvasSize.x) * 0.5f);
				result.x = result.x < 2f ? 0f : result.x;
				result.y = Mathf.Max(0f, Mathf.Abs(resolution.y - canvasSize.y) * 0.5f);
				result.y = result.y < 2f ? 0f : result.y;

				return result;
			}


			public static void SetCanvasCameraViewport(Camera camera)
			{
				Vector2 canvasSize = GetCanvasSize();
				Vector2 margin = GetMargin();

				float x = 0f;
				float w = 1f;
				if (margin.x > 1f) {
					w = (canvasSize.x - (margin.x * 2)) / canvasSize.x;
					x = (1f - w) * 0.5f;
				}

				float y = 0f;
				float h = 1f;
				if (margin.y > 1f) {
					h = (canvasSize.y - (margin.y * 2)) / canvasSize.y;
					y = (1f - h) * 0.5f;
				}

				camera.rect = new Rect(x, y, w, h);
			}
		}
	}
}