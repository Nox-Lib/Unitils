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
		private const string IS_TABLE_GENERATION_TARGET_FOLDERS = "IS_TABLE_GENERATION_TARGET_FOLDERS";

		private bool isError;

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

			this.isError = false;

			this.DrawPathField("inputFolder");
			this.DrawPathField("classGenerateFolder");
			this.DrawPathField("dataGenerateFolder");

			this.DrawWritableTableSettings();
			this.DrawGenerateTargets();

			EditorGUILayout.Space(10f);
			EditorGUILayout.BeginHorizontal();
			
			if (GUILayout.Button("Class Generate")) {
			}
			if (GUILayout.Button("Data Generate")) {
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
			EditorGUILayout.BeginHorizontal(GUI.skin.box);
			EditorGUI.indentLevel++;
			EditorGUIUtility.labelWidth = 60f;
			EditorGUILayout.PropertyField(this.serializedObject.FindProperty(propertyName), new GUIContent("Assets/"));
			EditorGUIUtility.labelWidth = defaultLabelWidth;
			EditorGUI.indentLevel--;
			EditorGUILayout.EndHorizontal();
		}

		private void DrawWritableTableSettings()
		{
			EditorGUILayout.Space(5f);

			SerializedProperty isWritableTableProperty = this.serializedObject.FindProperty("isWritableTable");
			isWritableTableProperty.boolValue = EditorGUILayout.ToggleLeft("Is Writable Table", isWritableTableProperty.boolValue);

			SerializedProperty writableTablePathMatchPatternProperty = this.serializedObject.FindProperty("writableTablePathMatchPattern");
			if (isWritableTableProperty.boolValue) {
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField(writableTablePathMatchPatternProperty, new GUIContent("Path Match Pattern"));
				EditorGUI.indentLevel--;
			}

			if (!string.IsNullOrEmpty(writableTablePathMatchPatternProperty.stringValue)) {
				try {
					Regex regex = new Regex(writableTablePathMatchPatternProperty.stringValue);
				}
				catch (Exception e) {
					string errorMessage = $"path match pattern error. {e.Message}";
					EditorGUILayout.HelpBox(errorMessage, MessageType.Error, true);
					this.isError = true;
				}
			}

			this.serializedObject.ApplyModifiedProperties();
		}

		private void DrawGenerateTargets()
		{
			if (this.isError) return;

			TableGeneratorData data = this.target as TableGeneratorData;

			string inputFolder = Path.Combine(Application.dataPath, data.InputFolder);
			if (!Directory.Exists(inputFolder)) return;

			List<string> inputTargets = Directory.GetDirectories(inputFolder, "*", SearchOption.AllDirectories).ToList();

			if (inputTargets.Count <= 0) return;

			List<string> readOnlyTables = new List<string>();
			List<string> writableTables = new List<string>();
			string matchPattern = data.WritableTablePathMatchPattern;

			foreach (string target in inputTargets) {
				string folder = target.Replace(inputFolder, "").TrimStart('/');
				if (data.IsWritableTable && !string.IsNullOrEmpty(matchPattern) && Regex.IsMatch(folder, matchPattern)) {
					writableTables.Add(folder);
				}
				else {
					readOnlyTables.Add(folder);
				}
			}

			EditorGUILayout.Space(10f);

			bool isOpen = EditorPrefs.GetBool(IS_TABLE_GENERATION_TARGET_FOLDERS);
			isOpen = EditorGUILayout.Foldout(isOpen, "Table Generation Target Folders");
			EditorPrefs.SetBool(IS_TABLE_GENERATION_TARGET_FOLDERS, isOpen);

			if (!isOpen) return;

			EditorGUILayout.BeginVertical(GUI.skin.box);

			if (readOnlyTables.Count > 0) {
				EditorGUILayout.LabelField("Read Only Tables");
				EditorGUILayout.BeginVertical(GUI.skin.box);
				EditorGUI.indentLevel++;
				readOnlyTables.ForEach(_ => EditorGUILayout.LabelField(_));
				EditorGUI.indentLevel--;
				EditorGUILayout.EndVertical();
			}
			if (writableTables.Count > 0) {
				EditorGUILayout.LabelField("Writable Tables");
				EditorGUILayout.BeginVertical(GUI.skin.box);
				EditorGUI.indentLevel++;
				writableTables.ForEach(_ => EditorGUILayout.LabelField(_));
				EditorGUI.indentLevel--;
				EditorGUILayout.EndVertical();
			}

			EditorGUILayout.EndVertical();
		}
	}
}