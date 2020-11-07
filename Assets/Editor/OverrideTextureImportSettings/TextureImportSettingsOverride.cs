using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Unitils
{
	public class TextureImportSettingsOverride : ScriptableObject
	{
		private static TextureImportSettingsOverride instance = null;
		public static TextureImportSettingsOverride Instance {
			get {
				return instance = instance ?? (instance = Resources.Load<TextureImportSettingsOverride>(DefineData.TEXTURE_IMPORT_SETTINGS_OVERRIDE));
			}
		}

		[Serializable]
		public class Configuration
		{
			#pragma warning disable 414
			[SerializeField, HideInInspector] private bool isInitialized = false;
			#pragma warning restore 414

			[SerializeField] private bool enabled = true;
			public bool Enabled => this.enabled;

			[SerializeField, HideInInspector] private bool isValid = true;
			public bool IsValid => this.isValid;

			[SerializeField] private TextureImportSettingsPreset preset;
			public TextureImportSettingsPreset Preset => this.preset;

			[SerializeField] private string pathMatchPattern;
			public string PathMatchPattern => this.pathMatchPattern;

			[SerializeField] private string pathIgnorePattern;
			public string PathIgnorePattern => this.pathIgnorePattern;
		}

		[SerializeField] private List<Configuration> configurations;
		public List<Configuration> Configurations => this.configurations;


		public static Configuration GetUseConfiguration(string assetPath)
		{
			Configuration useConfiguration = Instance.Configurations.FirstOrDefault(configuration =>
			{
				if (!configuration.Enabled || !configuration.IsValid || configuration.Preset == null) {
					return false;
				}
				if (!string.IsNullOrEmpty(configuration.PathMatchPattern) && !Regex.IsMatch(assetPath, configuration.PathMatchPattern)) {
					return false;
				}
				if (!string.IsNullOrEmpty(configuration.PathIgnorePattern) && Regex.IsMatch(assetPath, configuration.PathIgnorePattern)) {
					return false;
				}
				return true;
			});

			return useConfiguration;
		}


		[MenuItem("Unitils/OverrideTextureImportSettings/Selection")]
		[MenuItem("Assets/Unitils/OverrideTextureImportSettings/Selection")]
		private static void Run()
		{
			List<string> assetPaths = EditorTools.GetSelectionAssets(assetPath => {
				return (AssetImporter.GetAtPath(assetPath) as TextureImporter) != null && GetUseConfiguration(assetPath) != null;
			});

			if (assetPaths.Count <= 0) return;

			List<EditorGeneralIndicator.Task> tasks = new List<EditorGeneralIndicator.Task>();
			assetPaths.ForEach(assetPath => {
				tasks.Add(new EditorGeneralIndicator.Task(
					() => {
						Run(AssetImporter.GetAtPath(assetPath) as TextureImporter, GetUseConfiguration(assetPath));
						AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);
					},
					assetPath
				));
			});

			EditorGeneralIndicator.Show(
				"Override Texture Import Settings",
				tasks,
				() => {
					AssetDatabase.SaveAssets();
					AssetDatabase.Refresh();
					string logTitle = $"[OverrideTextureImportSettings - Selection] Results ({assetPaths.Count})";
					Debug.Log($"{logTitle} :\n-----\n{string.Join("\n", assetPaths)}\n-----");
				}
			);
		}


		public static void Run(TextureImporter textureImporter, Configuration configuration)
		{
			if (textureImporter == null) {
				throw new NullReferenceException("[TextureImportSettingsOverride] TextureImporter is null.");
			}
			if (configuration == null) {
				throw new NullReferenceException("[TextureImportSettingsOverride] Configuration is null.");
			}

			TextureImporterSettings importerSettings = new TextureImporterSettings();
			textureImporter.ReadTextureSettings(importerSettings);

			TextureImportSettingsPreset preset = configuration.Preset;
			CopyTextureImporterSettings(preset.ImporterSettings, importerSettings);

			TextureImporterPlatformSettings importerPlatformSettings = textureImporter.GetDefaultPlatformTextureSettings();
			CopyTextureImporterPlatformSettings(preset.DefaultPlatform.Settings, importerPlatformSettings);
			textureImporter.SetPlatformTextureSettings(importerPlatformSettings);

			foreach (TextureImportPlatformSettingsGroup platformGroup in preset.PlatformGroups) {
				importerPlatformSettings = textureImporter.GetPlatformTextureSettings(platformGroup.Settings.name);
				CopyTextureImporterPlatformSettings(platformGroup.Settings, importerPlatformSettings);
				textureImporter.SetPlatformTextureSettings(importerPlatformSettings);
			}

			textureImporter.SetTextureSettings(importerSettings);
			textureImporter.SaveAndReimport();
		}


		private static void CopyTextureImporterSettings(TextureImporterSettings source, TextureImporterSettings destination)
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

		private static void CopyTextureImporterPlatformSettings(TextureImporterPlatformSettings source, TextureImporterPlatformSettings destination)
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