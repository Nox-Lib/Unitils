using UnityEngine;

namespace Unitils
{
	public class SafeAreaBounds : MonoBehaviour
	{
		#if UNITY_EDITOR
		[SerializeField, Label("Is Emulate iPhoneX")]
		private bool isEmulateIPhoneX = false;
		#endif

		private RectTransform target = null;
		private RectTransform Target { get { return this.target ??= GetComponent<RectTransform>(); } }

		private void Awake()
		{
			this.Apply();
		}

		private void Apply()
		{
			if (this.Target == null) return;

			Vector2 screenSize = new Vector2(Screen.width, Screen.height);
			Rect safeArea = Screen.safeArea;

			#if UNITY_EDITOR
			if (this.CanEmulate) {
				screenSize = this.GetEmulateScreenSize();
				safeArea = this.GetEmulateSafeArea();
			}
			#endif

			Vector2 anchorMin = safeArea.position;
			Vector2 anchorMax = safeArea.position + safeArea.size;
			anchorMin.x /= screenSize.x;
			anchorMin.y /= screenSize.y;
			anchorMax.x /= screenSize.x;
			anchorMax.y /= screenSize.y;
			this.target.anchorMin = anchorMin;
			this.target.anchorMax = anchorMax;
		}


		#if UNITY_EDITOR

		private struct Margin
		{
			public float top;
			public float bottom;
			public float left;
			public float right;
		}

		private bool CanEmulate {
			get {
				bool isIPhoneX = false;
				isIPhoneX |= Screen.width == 1125 && Screen.height == 2436;
				isIPhoneX |= Screen.width == 2436 && Screen.height == 1125;
				return this.isEmulateIPhoneX && isIPhoneX;
			}
		}

		private Vector2 GetEmulateScreenSize()
		{
			return Screen.width > Screen.height ? new Vector2(812f, 375f) : new Vector2(375f, 812f);
		}

		private Rect GetEmulateSafeArea()
		{
			Rect rect = new Rect();

			if (Screen.width > Screen.height) {
				Margin margin = new Margin { top = 0f, bottom = 21f, left = 44f, right = 44f };
				rect.position = new Vector2(margin.left, margin.bottom);
				rect.size = new Vector2(724f, 354f);
			}
			else {
				Margin margin = new Margin { top = 44f, bottom = 34f, left = 0f, right = 0f };
				rect.position = new Vector2(margin.left, margin.bottom);
				rect.size = new Vector2(375f, 734f);
			}

			return rect;
		}
		#endif
	}
}