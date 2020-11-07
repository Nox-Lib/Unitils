using UnityEngine;
using UnityEditor;
using System.IO;

namespace Unitils
{
	public static class UnitilsSetup
	{
		private static readonly string rootFolder = Path.Combine(Application.dataPath, "UnitilsApps");

		[MenuItem("Unitils/Setup")]
		private static void Setup()
		{
			CreateFolder(rootFolder);
			GenerateSystemData();
			GenerateTextureImportSettingsOverride();

			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}


		private static void GenerateSystemData()
		{
			string folderPath = Path.Combine(rootFolder, "Resources/Data");
			CreateFolder(folderPath);

			string filePath = Path.Combine(folderPath, "SystemData.asset");
			if (File.Exists(filePath)) return;
			ScriptableObjectToAsset.Create<SystemData>(EditorTools.ToAssetPath(filePath));
		}

		private static void GenerateTextureImportSettingsOverride()
		{
			string folderPath = Path.Combine(rootFolder, "Resources/Data/OverrideTextureImportSettings");
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
			if (Directory.Exists(folderPath)) return;
			Directory.CreateDirectory(folderPath);
		}
	}
}