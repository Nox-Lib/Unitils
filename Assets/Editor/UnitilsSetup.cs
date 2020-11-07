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
			GenerateTextureImporterAutoSettings();

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

		private static void GenerateTextureImporterAutoSettings()
		{
			string folderPath = Path.Combine(rootFolder, "Resources/Data/OverrideTextureImportSettings");
			CreateFolder(folderPath);

			string filePath = Path.Combine(folderPath, "Configurations.asset");
			if (!File.Exists(filePath)) {
				ScriptableObjectToAsset.Create<TextureImportSettingsOverride>(EditorTools.ToAssetPath(filePath));
			}

			filePath = Path.Combine(folderPath, "Template.asset");
			if (!File.Exists(filePath)) {
				ScriptableObjectToAsset.Create<TextureImportSettingsTemplate>(EditorTools.ToAssetPath(filePath));
			}
		}


		private static void CreateFolder(string folderPath)
		{
			if (Directory.Exists(folderPath)) return;
			Directory.CreateDirectory(folderPath);
		}
	}
}