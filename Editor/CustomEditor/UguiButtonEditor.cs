using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace Unitils
{
	[CustomEditor(typeof(UguiButton))]
	public class UguiButtonEditor : Editor
	{
		private const string IS_OPEN_UGUI_BUTTON_ANIMATION = "IS_OPEN_UGUI_BUTTON_ANIMATION";
		private const string IS_OPEN_UGUI_BUTTON_SOUND = "IS_OPEN_UGUI_BUTTON_SOUND";

		private static readonly List<Define.ButtonTrigger> SUPPORTED_TRIGGERS = new List<Define.ButtonTrigger>
		{
			Define.ButtonTrigger.Click,
			Define.ButtonTrigger.Down
		};

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

			UguiButton uguiButton = this.target as UguiButton;
			SerializedProperty serializedTriggerIndex = this.serializedObject.FindProperty("triggerIndex");
			int triggerIndex = serializedTriggerIndex.intValue;

			if (triggerIndex < 0) {
				serializedTriggerIndex.intValue = triggerIndex = 0;
				uguiButton.Trigger = SUPPORTED_TRIGGERS[0];
			}

			EditorGUILayout.BeginVertical();
			GUILayout.Space(2f);
			EditorGUILayout.PropertyField(this.serializedObject.FindProperty("isInteractable"), new GUIContent("Interactable"));

			triggerIndex = EditorGUILayout.Popup("Trigger", serializedTriggerIndex.intValue, SUPPORTED_TRIGGERS.Select(x => x.ToString()).ToArray());
			serializedTriggerIndex.intValue = triggerIndex;
			uguiButton.Trigger = SUPPORTED_TRIGGERS[triggerIndex];

			if (uguiButton.Trigger == Define.ButtonTrigger.Down) {
				EditorGUILayout.PropertyField(this.serializedObject.FindProperty("triggerDownType"), new GUIContent("Down Type"));
				if (uguiButton.DownType != UguiButton.TriggerDownType.DownOnce) {
					EditorGUILayout.PropertyField(this.serializedObject.FindProperty("interval"));
				}
				if (uguiButton.DownType == UguiButton.TriggerDownType.DownLong) {
					EditorGUILayout.PropertyField(this.serializedObject.FindProperty("canAlsoClick"));
				}
				if (uguiButton.DownType == UguiButton.TriggerDownType.DownKeepAcceleration) {
					EditorGUILayout.PropertyField(this.serializedObject.FindProperty("accelerationTime"));
					EditorGUILayout.PropertyField(this.serializedObject.FindProperty("acceleratedInterval"));
				}
				if (uguiButton.DownType == UguiButton.TriggerDownType.DownKeep || uguiButton.DownType == UguiButton.TriggerDownType.DownKeepAcceleration) {
					EditorGUILayout.PropertyField(this.serializedObject.FindProperty("isDontInvokeAtFirst"), new GUIContent("Don't Invoke At First"));
					EditorGUILayout.PropertyField(this.serializedObject.FindProperty("isPlaySoundOnEveryInvoke"), new GUIContent("Play Sound On Every Invoke"));
				}
			}
			GUILayout.Space(2f);
			EditorGUILayout.EndVertical();

			bool isOpenAnimation = EditorPrefs.GetBool(IS_OPEN_UGUI_BUTTON_ANIMATION);
			isOpenAnimation = EditorGUILayout.Foldout(isOpenAnimation, "Animation");
			EditorPrefs.SetBool(IS_OPEN_UGUI_BUTTON_ANIMATION, isOpenAnimation);

			if (isOpenAnimation) {
				EditorGUILayout.BeginVertical(GUI.skin.box);
				SerializedProperty enabledAnimationProperty = this.serializedObject.FindProperty("isEnabledAnimation");
				EditorGUILayout.PropertyField(enabledAnimationProperty, new GUIContent("Enabled"));
				if (enabledAnimationProperty.boolValue) {
					EditorGUILayout.PropertyField(this.serializedObject.FindProperty("duration"));
					GUILayout.Space(4f);
					EditorGUILayout.PropertyField(this.serializedObject.FindProperty("normalScale"));
					EditorGUILayout.PropertyField(this.serializedObject.FindProperty("enteredScale"));
					EditorGUILayout.PropertyField(this.serializedObject.FindProperty("pressedScale"));
					GUILayout.Space(4f);
					EditorGUILayout.PropertyField(this.serializedObject.FindProperty("normalColor"));
					EditorGUILayout.PropertyField(this.serializedObject.FindProperty("enteredColor"));
					EditorGUILayout.PropertyField(this.serializedObject.FindProperty("pressedColor"));
					EditorGUILayout.PropertyField(this.serializedObject.FindProperty("disabledColor"));
				}
				EditorGUILayout.EndVertical();
			}

			bool isOpenSound = EditorPrefs.GetBool(IS_OPEN_UGUI_BUTTON_SOUND);
			isOpenSound = EditorGUILayout.Foldout(isOpenSound, "Sound");
			EditorPrefs.SetBool(IS_OPEN_UGUI_BUTTON_SOUND, isOpenSound);

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
			if (uguiButton.Trigger == Define.ButtonTrigger.Down && uguiButton.DownType == UguiButton.TriggerDownType.DownLong) {
				EditorGUILayout.PropertyField(this.serializedObject.FindProperty("onLongPressedEvent"));
			}
			EditorGUILayout.EndVertical();

			this.serializedObject.ApplyModifiedProperties();
		}
	}
}