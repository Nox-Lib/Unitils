using UnityEngine;
using UnityEditor;

namespace Unitils
{
	[CustomEditor(typeof(UguiClickDetector))]
	public class UguiClickDetectorEditor : Editor
	{
		private const string IS_OPEN_UGUI_CLICK_EVENT_DETECTOR_SOUND = "IS_OPEN_UGUI_CLICK_DETECTOR_SOUND";

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

			UguiClickDetector detector = this.target as UguiClickDetector;
			detector.Trigger = Define.ButtonTrigger.Click;

			EditorGUILayout.BeginVertical();
			GUILayout.Space(5f);
			EditorGUILayout.PropertyField(this.serializedObject.FindProperty("isInteractable"), new GUIContent("Interactable"));
			EditorGUILayout.EndVertical();

			bool isOpenSound = EditorPrefs.GetBool(IS_OPEN_UGUI_CLICK_EVENT_DETECTOR_SOUND);
			isOpenSound = EditorGUILayout.Foldout(isOpenSound, "Sound");
			EditorPrefs.SetBool(IS_OPEN_UGUI_CLICK_EVENT_DETECTOR_SOUND, isOpenSound);

			if (isOpenSound) {
				EditorGUILayout.BeginVertical(GUI.skin.box);
				EditorGUILayout.PropertyField(this.serializedObject.FindProperty("soundType"));
				EditorGUILayout.PropertyField(this.serializedObject.FindProperty("volume"));
				EditorGUILayout.PropertyField(this.serializedObject.FindProperty("pan"));
				EditorGUILayout.EndVertical();
			}

			EditorGUILayout.BeginVertical();
			GUILayout.Space(5f);
			EditorGUILayout.PropertyField(this.serializedObject.FindProperty("onEvent"));
			EditorGUILayout.EndVertical();

			this.serializedObject.ApplyModifiedProperties();
		}
	}
}