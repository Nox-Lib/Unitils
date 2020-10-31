using UnityEngine;
using UnityEditor;

namespace Unitils
{
	[CustomEditor(typeof(SystemData))]
	public class SystemDataEditor : Editor
	{
		private const string IS_SYSTEM_DATA_SYSTEM = "IS_SYSTEM_DATA_SYSTEM";
		private const string IS_SYSTEM_DATA_SCREEN = "IS_SYSTEM_DATA_SCREEN";
		private const string IS_SYSTEM_DATA_FRAMEWORK = "IS_SYSTEM_DATA_FRAMEWORK";

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

			SystemData systemData = this.target as SystemData;

			#region System

			bool isOpenSystem = EditorPrefs.GetBool(IS_SYSTEM_DATA_SYSTEM);
			isOpenSystem = EditorGUILayout.Foldout(isOpenSystem, "System");
			EditorPrefs.SetBool(IS_SYSTEM_DATA_SYSTEM, isOpenSystem);

			if (isOpenSystem) {
				EditorGUILayout.BeginVertical(GUI.skin.box);
				EditorGUILayout.PropertyField(this.serializedObject.FindProperty("frameRate"));
				EditorGUILayout.EndVertical();
			}
			#endregion

			#region Screen
			GUILayout.Space(5f);

			bool isOpenScreen = EditorPrefs.GetBool(IS_SYSTEM_DATA_SCREEN);
			isOpenScreen = EditorGUILayout.Foldout(isOpenScreen, "Screen");
			EditorPrefs.SetBool(IS_SYSTEM_DATA_SCREEN, isOpenScreen);

			if (isOpenScreen) {
				EditorGUILayout.BeginVertical(GUI.skin.box);
				EditorGUILayout.PropertyField(this.serializedObject.FindProperty("screenMode"));

				switch (systemData.ScreenMode) {
				case Define.System.ScreenMode.Fixed:
					EditorGUILayout.PropertyField(this.serializedObject.FindProperty("baseScreenSize"));
					break;
				case Define.System.ScreenMode.Expand:
					EditorGUILayout.PropertyField(this.serializedObject.FindProperty("baseScreenSize"));
					EditorGUILayout.PropertyField(this.serializedObject.FindProperty("minScreenSize"));
					EditorGUILayout.PropertyField(this.serializedObject.FindProperty("maxScreenSize"));
					break;
				}

				if (systemData.ScreenMode == Define.System.ScreenMode.Expand) {
					bool isInRangeW = Utils.Math.InRange(systemData.BaseScreenSize.x, systemData.MinScreenSize.x, systemData.MaxScreenSize.x);
					bool isInRangeH = Utils.Math.InRange(systemData.BaseScreenSize.y, systemData.MinScreenSize.y, systemData.MaxScreenSize.y);
					if (!isInRangeW || !isInRangeH) {
						EditorGUILayout.HelpBox("BaseScreenSizeはMinScreenSizeとMaxScreenSizeの範囲内で設定してください。", MessageType.Warning);
					}
				}

				EditorGUILayout.PropertyField(this.serializedObject.FindProperty("isOutsideScreenMask"));
				if (systemData.IsOutsideScreenMask) {
					EditorGUILayout.PropertyField(this.serializedObject.FindProperty("screenMaskColor"));
				}
				EditorGUILayout.EndVertical();
			}
			#endregion

			this.serializedObject.ApplyModifiedProperties();
		}
	}
}