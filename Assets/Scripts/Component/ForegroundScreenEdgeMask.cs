using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Unitils
{
	public class ForegroundScreenEdgeMask : MonoBehaviour
	{
		[SerializeField] private Image maskLeft = null;
		[SerializeField] private Image maskRight = null;
		[SerializeField] private Image maskTop = null;
		[SerializeField] private Image maskBottom = null;

		[SerializeField] private UguiColorGroup colorGroup = null;

		private void Start()
		{
			this.Apply();
		}

		private void Apply()
		{
			SystemData systemData = SystemData.Instance;
			Vector2 maskSize = Utils.Screen.GetMargin(systemData);

			this.maskLeft.rectTransform.SetSizeDeltaX(maskSize.x);
			this.maskRight.rectTransform.SetSizeDeltaX(maskSize.x);
			this.maskTop.rectTransform.SetSizeDeltaY(maskSize.y);
			this.maskBottom.rectTransform.SetSizeDeltaY(maskSize.y);

			this.colorGroup.SetColor(systemData.ScreenEdgeMaskColor);
		}


		#if UNITY_EDITOR

		private int activeWidth;
		private int activeHieght;

		private void Update()
		{
			SystemData systemData = SystemData.Instance;
			bool isApply = false;

			if (this.activeWidth != Screen.width) {
				this.activeWidth = Screen.width;
				isApply = true;
			}
			if (this.activeHieght != Screen.height) {
				this.activeHieght = Screen.height;
				isApply = true;
			}
			this.activeWidth = Screen.width;
			this.activeHieght = Screen.height;

			isApply |= this.colorGroup.Current != systemData.ScreenEdgeMaskColor;
			if (isApply) this.Apply();
		}

		[CustomEditor(typeof(ForegroundScreenEdgeMask))]
		private class InspectorEditor : Editor
		{
			public override void OnInspectorGUI()
			{
				base.OnInspectorGUI();

				EditorGUILayout.Space();

				if (GUILayout.Button("Apply")) {
					ForegroundScreenEdgeMask instance = this.target as ForegroundScreenEdgeMask;
					instance?.Apply();
				}
			}
		}
		#endif
	}
}