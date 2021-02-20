using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace Unitils
{
	[CustomEditor(typeof(TableGeneratorData))]
	public class TableGeneratorDataEditor : Editor
	{
		private bool isError;

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

			TableGeneratorData data = this.target as TableGeneratorData;

			this.isError = false;

			this.DrawPathField("inputFolder");
			this.DrawPathField("classOutputFolder");
			this.DrawPathField("dataOutputFolder");
			this.serializedObject.ApplyModifiedProperties();

			this.DrawEncryption(data);

			this.DrawGenerationTargets(data);

			EditorGUILayout.Space(10f);
			EditorGUILayout.BeginHorizontal();
			
			if (GUILayout.Button("Generate Class")) {
				if (!this.isError) TableGenerator.GenerateClass(data);
			}
			if (GUILayout.Button("Generate Data")) {
				if (!this.isError) TableGenerator.GenerateData(data);
			}
			EditorGUILayout.EndHorizontal();

			this.serializedObject.ApplyModifiedProperties();
		}

		private void DrawPathField(string propertyName)
		{
			float defaultLabelWidth = EditorGUIUtility.labelWidth;

			IEnumerable<string> words = Utils.Text.SplitByUpper(propertyName).Select(_ => Utils.Text.ToUpper(_, 0));
			string labelName = string.Join(" ", words);

			EditorGUILayout.LabelField(labelName);
			EditorGUILayout.BeginVertical(GUI.skin.box);
			EditorGUI.indentLevel++;
			EditorGUIUtility.labelWidth = 60f;
			EditorGUILayout.PropertyField(this.serializedObject.FindProperty(propertyName), new GUIContent("Assets/"));
			EditorGUIUtility.labelWidth = defaultLabelWidth;
			EditorGUI.indentLevel--;
			EditorGUILayout.EndVertical();
		}

		private void DrawEncryption(TableGeneratorData data)
		{
			EditorGUILayout.Space(5f);

			EditorGUILayout.LabelField("Data Encryption Settings");
			EditorGUILayout.BeginVertical(GUI.skin.box);

			SerializedProperty isToMD5Property = this.serializedObject.FindProperty("isDataFileNameToMD5");
			isToMD5Property.boolValue = EditorGUILayout.Toggle("File Name To MD5", isToMD5Property.boolValue);

			EditorGUILayout.PropertyField(this.serializedObject.FindProperty("encryptionType"));
			if (data.EncryptionType == Define.EncryptionType.AES) {
				EditorGUILayout.PropertyField(this.serializedObject.FindProperty("encryptAesKey"), new GUIContent("AES Key"));
				EditorGUILayout.PropertyField(this.serializedObject.FindProperty("encryptAesIv"), new GUIContent("AES IV"));
			}

			EditorGUILayout.EndVertical();
		}

		private void DrawGenerationTargets(TableGeneratorData data)
		{
			string inputFolder = Path.Combine(Application.dataPath, data.InputFolder);
			if (!Directory.Exists(inputFolder)) return;

			string[] targets = Directory.GetDirectories(inputFolder, "*", SearchOption.AllDirectories);
			Array.Sort(targets);

			if (targets.Length <= 0) return;

			bool dontLayout = targets.Length != data.Folders.Count;
			if (!dontLayout) {
				for (int i = 0; i < targets.Length && !dontLayout; i++) {
					dontLayout = targets[i] != data.Folders[i].path;
				}
			}

			if (Event.current.type == EventType.Repaint) {
				List<TableGeneratorData.FolderData> folders = new List<TableGeneratorData.FolderData>(data.Folders);
				data.Folders.Clear();
				for (int i = 0; i < targets.Length; i++) {
					TableGeneratorData.FolderData folderData = folders.FirstOrDefault(_ => _.path == targets[i]);
					if (folderData == null) folderData = new TableGeneratorData.FolderData { path = targets[i] };
					data.Folders.Add(folderData);
				}
				folders = data.Folders;
				if (targets.Length > folders.Count) {
					folders.RemoveRange(targets.Length - 1, targets.Length - folders.Count);
				}
				if (dontLayout) return;
			}
			else {
				if (dontLayout) return;
			}

			EditorGUILayout.Space(5f);

			EditorGUILayout.LabelField("Table Generation Targets");
			EditorGUILayout.BeginVertical(GUI.skin.box);

			SerializedProperty foldersProperty = this.serializedObject.FindProperty("folders");

			for (int i = 0; i < targets.Length; i++) {
				SerializedProperty folderProperty = foldersProperty.GetArrayElementAtIndex(i);
				string folder = targets[i].Replace(inputFolder, "").TrimStart('/');

				string[] files = Directory.GetFiles(targets[i], "*.csv");
				string folderLabel = $"{folder} ({files.Length} files)";

				SerializedProperty enabledProperty = folderProperty.FindPropertyRelative("enabled");
				enabledProperty.boolValue = EditorGUILayout.ToggleLeft(folderLabel, enabledProperty.boolValue);

				folderProperty.FindPropertyRelative("path").stringValue = targets[i];
				folderProperty.FindPropertyRelative("folderName").stringValue = folder;

				if (!enabledProperty.boolValue) continue;

				EditorGUILayout.BeginVertical(GUI.skin.box);

				SerializedProperty isWritableTableProperty = folderProperty.FindPropertyRelative("isWritableTable");
				isWritableTableProperty.boolValue = EditorGUILayout.Toggle("Is Writable Table", isWritableTableProperty.boolValue);

				SerializedProperty separatorProperty = folderProperty.FindPropertyRelative("classNameSeparator");
				EditorGUILayout.PropertyField(separatorProperty);
				if (separatorProperty.stringValue.Length > 1) {
					separatorProperty.stringValue = separatorProperty.stringValue.Substring(0, 1);
				}

				SerializedProperty classNameEraserProperty = folderProperty.FindPropertyRelative("classNameEraser");
				EditorGUILayout.PropertyField(classNameEraserProperty);
				if (!string.IsNullOrEmpty(classNameEraserProperty.stringValue)) {
					try {
						Regex regex = new Regex(classNameEraserProperty.stringValue);
					}
					catch (Exception e) {
						this.isError = true;
						EditorGUILayout.HelpBox($"path match pattern error. {e.Message}", MessageType.Error, true);
					}
				}

				SerializedProperty classNameFormatProperty = folderProperty.FindPropertyRelative("classNameFormat");
				EditorGUILayout.PropertyField(classNameFormatProperty);
				if (string.IsNullOrEmpty(classNameFormatProperty.stringValue)) {
					classNameFormatProperty.stringValue = "*";
				}
				else if (classNameFormatProperty.stringValue.Count(_ => _ == '*') != 1) {
					this.isError = true;
					EditorGUILayout.HelpBox("only one '*' is required", MessageType.Error, true);
				}

				EditorGUILayout.EndVertical();
			}

			EditorGUILayout.EndVertical();
		}
	}
}