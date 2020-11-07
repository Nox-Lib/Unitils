using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Unitils
{
	public class TextureImportPostprocessor : AssetPostprocessor
	{
		private void OnPostprocessTexture(Texture2D texture)
		{
			List<TextureImportSettingsOverride.Configuration> configurations = TextureImportSettingsOverride.Instance.Configurations;

			if (configurations != null && configurations.Count <= 0) return;
			if (AssetDatabase.LoadAssetAtPath<Texture2D>(this.assetPath)) return;

			TextureImportSettingsOverride.Configuration useConfiguration = configurations.FirstOrDefault(configuration =>
			{
				if (!configuration.Enabled || !configuration.IsValid || configuration.Template == null) {
					return false;
				}
				if (!string.IsNullOrEmpty(configuration.PathMatchPattern) && !Regex.IsMatch(this.assetPath, configuration.PathMatchPattern)) {
					return false;
				}
				if (!string.IsNullOrEmpty(configuration.PathIgnorePattern) && Regex.IsMatch(this.assetPath, configuration.PathIgnorePattern)) {
					return false;
				}
				return true;
			});

			if (useConfiguration == null) return;

			this.SetTextureImporterSettings(this.assetImporter as TextureImporter, useConfiguration);
		}


		private void SetTextureImporterSettings(TextureImporter textureImporter, TextureImportSettingsOverride.Configuration configuration)
		{
			TextureImporterSettings importerSettings = new TextureImporterSettings();
			textureImporter.ReadTextureSettings(importerSettings);

			TextureImportSettingsTemplate template = configuration.Template;
			this.CopyTextureImporterSettings(template.ImporterSettings, importerSettings);

			TextureImporterPlatformSettings importerPlatformSettings = textureImporter.GetDefaultPlatformTextureSettings();
			this.CopyTextureImporterPlatformSettings(template.DefaultPlatform.Settings, importerPlatformSettings);
			textureImporter.SetPlatformTextureSettings(importerPlatformSettings);

			foreach (TextureImportPlatformSettingsGroup platformGroup in template.PlatformGroups) {
				importerPlatformSettings = textureImporter.GetPlatformTextureSettings(platformGroup.Settings.name);
				this.CopyTextureImporterPlatformSettings(platformGroup.Settings, importerPlatformSettings);
				textureImporter.SetPlatformTextureSettings(importerPlatformSettings);
			}

			textureImporter.SetTextureSettings(importerSettings);
			textureImporter.SaveAndReimport();
		}


		private void CopyTextureImporterSettings(TextureImporterSettings source, TextureImporterSettings destination)
		{
			destination.alphaSource = source.alphaSource;
			destination.mipmapFilter = source.mipmapFilter;
			destination.mipmapEnabled = source.mipmapEnabled;
			destination.fadeOut = source.fadeOut;
			destination.borderMipmap = source.borderMipmap;
			destination.mipMapsPreserveCoverage = source.mipMapsPreserveCoverage;
			destination.alphaTestReferenceValue = source.alphaTestReferenceValue;
			destination.mipmapFadeDistanceStart = source.mipmapFadeDistanceStart;
			destination.mipmapFadeDistanceEnd = source.mipmapFadeDistanceEnd;
			destination.readable = source.readable;
			destination.streamingMipmaps = source.streamingMipmaps;
			destination.streamingMipmapsPriority = source.streamingMipmapsPriority;
			destination.npotScale = source.npotScale;
			destination.sRGBTexture = source.sRGBTexture;
			destination.spriteMode = source.spriteMode;
			destination.spriteExtrude = source.spriteExtrude;
			destination.spriteMeshType = source.spriteMeshType;
			destination.spriteAlignment = source.spriteAlignment;
			destination.spritePivot = source.spritePivot;
			destination.spritePixelsPerUnit = source.spritePixelsPerUnit;
			destination.spriteGenerateFallbackPhysicsShape = source.spriteGenerateFallbackPhysicsShape;
			destination.alphaIsTransparency = source.alphaIsTransparency;
			destination.textureType = source.textureType;
			destination.textureShape = source.textureShape;
			destination.filterMode = source.filterMode;
			destination.aniso = source.aniso;
			destination.wrapMode = source.wrapMode;
			destination.wrapModeU = source.wrapModeU;
			destination.wrapModeV = source.wrapModeV;
			destination.wrapModeW = source.wrapModeW;
		}

		private void CopyTextureImporterPlatformSettings(TextureImporterPlatformSettings source, TextureImporterPlatformSettings destination)
		{
			destination.overridden = source.overridden;
			destination.maxTextureSize = source.maxTextureSize;
			destination.resizeAlgorithm = source.resizeAlgorithm;
			destination.format = source.format;
			destination.textureCompression = source.textureCompression;
			destination.crunchedCompression = source.crunchedCompression;
			destination.compressionQuality = source.compressionQuality;
			destination.allowsAlphaSplitting = source.allowsAlphaSplitting;
			destination.androidETC2FallbackOverride = source.androidETC2FallbackOverride;
		}
	}
}