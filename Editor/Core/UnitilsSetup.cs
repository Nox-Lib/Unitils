using System;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace Unitils
{
	public static class UnitilsSetup
	{
		private static readonly string rootFolder = Path.Combine(Application.dataPath, "UnitilsApps");

		[MenuItem("Unitils/Setup")]
		private static void Setup()
		{
			CreateFolder(rootFolder);

			GenerateScriptableObjectData(
				(typeof(SystemData), false),
				(typeof(ButtonSoundData), false),
				(typeof(TableGeneratorData), true)
			);

			GenerateTextureImportSettingsOverride();

			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}

		private static void GenerateScriptableObjectData(params (Type type, bool isEditor)[] targets)
		{
			for (int i = 0; i < targets.Length; i++) {
				string folderName = targets[i].isEditor ? "Editor/Data" : "Resources/Data";
				string folderPath = Path.Combine(rootFolder, folderName);
				CreateFolder(folderPath);

				Type type = targets[i].type;
				string filePath = Path.Combine(folderPath, $"{type.Name}.asset");
				if (!type.IsSubclassOf(typeof(ScriptableObject)) || File.Exists(filePath)) continue;
				ScriptableObjectToAsset.Create(EditorTools.ToAssetPath(filePath), type);
			}
		}

		private static void GenerateTextureImportSettingsOverride()
		{
			string folderPath = Path.Combine(rootFolder, "Editor/Data/OverrideTextureImportSettings");
			CreateFolder(folderPath);

			string filePath = Path.Combine(folderPath, "Configurations.asset");
			if (!File.Exists(filePath)) {
				ScriptableObjectToAsset.Create<TextureImportSettingsOverride>(EditorTools.ToAssetPath(filePath));
			}

			filePath = Path.Combine(folderPath, "Preset.asset");
			if (!File.Exists(filePath)) {
				ScriptableObjectToAsset.Create<TextureImportSettingsPreset>(EditorTools.ToAssetPath(filePath));
			}
		}


		private static void CreateFolder(string folderPath)
		{
			if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
		}
	}
}