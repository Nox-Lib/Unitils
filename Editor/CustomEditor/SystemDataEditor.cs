using UnityEngine;
using UnityEditor;

namespace Unitils
{
	[CustomEditor(typeof(SystemData))]
	public class SystemDataEditor : Editor
	{
		private const string IS_SYSTEM_DATA_SYSTEM = "IS_SYSTEM_DATA_SYSTEM";
		private const string IS_SYSTEM_DATA_SCREEN = "IS_SYSTEM_DATA_SCREEN";

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
				case Define.ScreenMode.Fixed:
					EditorGUILayout.PropertyField(this.serializedObject.FindProperty("baseScreenSize"));
					break;
				case Define.ScreenMode.Expand:
					EditorGUILayout.PropertyField(this.serializedObject.FindProperty("baseScreenSize"));
					EditorGUILayout.PropertyField(this.serializedObject.FindProperty("minScreenSize"));
					EditorGUILayout.PropertyField(this.serializedObject.FindProperty("maxScreenSize"));
					break;
				}

				if (systemData.ScreenMode == Define.ScreenMode.Expand) {
					bool isInRangeW = Utils.Math.InRange(systemData.BaseScreenSize.x, systemData.MinScreenSize.x, systemData.MaxScreenSize.x);
					bool isInRangeH = Utils.Math.InRange(systemData.BaseScreenSize.y, systemData.MinScreenSize.y, systemData.MaxScreenSize.y);
					if (!isInRangeW || !isInRangeH) {
						EditorGUILayout.HelpBox("BaseScreenSize は MinScreenSize と MaxScreenSize の範囲内で設定してください。", MessageType.Warning);
					}
				}

				EditorGUILayout.PropertyField(this.serializedObject.FindProperty("isScreenEdgeMask"), new GUIContent("Is Edge Mask"));
				GUI.enabled = systemData.IsScreenEdgeMask;
				EditorGUILayout.PropertyField(this.serializedObject.FindProperty("screenEdgeMaskColor"), new GUIContent("Edge Mask Color"));
				GUI.enabled = true;
				EditorGUILayout.EndVertical();
			}
			#endregion

			this.serializedObject.ApplyModifiedProperties();
		}
	}
}