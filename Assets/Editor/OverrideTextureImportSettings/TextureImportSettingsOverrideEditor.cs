using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Unitils
{
	[CustomEditor(typeof(TextureImportSettingsOverride))]
	public class TextureImportSettingsOverrideEditor : Editor
	{
		private TextureImportSettingsOverride importSettingsOverride;
		private List<string> errorMessages;

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			this.serializedObject.Update();

			this.Validation();
			if (this.errorMessages.Count >= 1) {
				EditorGUILayout.HelpBox(string.Join("\n", this.errorMessages), MessageType.Error, true);
			}

			this.serializedObject.ApplyModifiedProperties();
		}

		private void OnEnable()
		{
			this.importSettingsOverride = this.target as TextureImportSettingsOverride;
			this.errorMessages = new List<string>();
		}

		private void Validation()
		{
			if (this.importSettingsOverride.Configurations == null) return;
			this.errorMessages.Clear();

			for (int i = 0; i < this.importSettingsOverride.Configurations.Count; i++) {
				TextureImportSettingsOverride.Configuration configuration = this.importSettingsOverride.Configurations[i];

				SerializedProperty configurationProperty = this.serializedObject.FindProperty("configurations").GetArrayElementAtIndex(i);
				SerializedProperty isInitializedProperty = configurationProperty.FindPropertyRelative("isInitialized");
				SerializedProperty enabledProperty = configurationProperty.FindPropertyRelative("enabled");
				SerializedProperty isValidProperty = configurationProperty.FindPropertyRelative("isValid");

				if (!isInitializedProperty.boolValue) {
					isInitializedProperty.boolValue = true;
					enabledProperty.boolValue = true;
				}
				if (!enabledProperty.boolValue) continue;

				if (!string.IsNullOrEmpty(configuration.PathMatchPattern)) {
					try {
						Regex regex = new Regex(configuration.PathMatchPattern);
					}
					catch (Exception e) {
						isValidProperty.boolValue = false;
						this.errorMessages.Add($"Configurations Element[{i}] is path match pattern error. {e.Message}");
					}
				}

				if (!string.IsNullOrEmpty(configuration.PathIgnorePattern)) {
					try {
						Regex regex = new Regex(configuration.PathIgnorePattern);
					}
					catch (Exception e) {
						isValidProperty.boolValue = false;
						this.errorMessages.Add($"Configurations Element[{i}] is path ignore pattern error. {e.Message}");
					}
				}

				isValidProperty.boolValue = true;
			}
		}
	}
}