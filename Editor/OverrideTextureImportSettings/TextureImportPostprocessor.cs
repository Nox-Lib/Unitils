using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Unitils
{
	public class TextureImportPostprocessor : AssetPostprocessor
	{
		private void OnPostprocessTexture(Texture2D texture)
		{
			List<TextureImportSettingsOverride.Configuration> configurations = TextureImportSettingsOverride.Instance.Configurations;

			if (configurations != null && configurations.Count <= 0) return;
			if (AssetDatabase.LoadAssetAtPath<Texture2D>(this.assetPath)) return;

			TextureImportSettingsOverride.Configuration useConfiguration = TextureImportSettingsOverride.GetUseConfiguration(this.assetPath);

			if (useConfiguration == null) return;

			TextureImportSettingsOverride.Run(this.assetImporter as TextureImporter, useConfiguration);
		}
	}
}