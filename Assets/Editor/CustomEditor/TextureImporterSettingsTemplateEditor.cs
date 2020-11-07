using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Unitils
{
	[CustomEditor(typeof(TextureImporterSettingsTemplate))]
	public class TextureImporterSettingsTemplateEditor : Editor
	{
		private const string TEXTURE_IMPORTER_SHOW_ADVANCED = "TextureImporterShowAdvanced";
		private const string TEXTURE_IMPORTER_PLATFORM_SHOW_DEFAULT = "TextureImporterPlatformShowDefault";

		private readonly TextureImporterType[] enabledTextureImporterTypes =
		{
			TextureImporterType.Default,
			TextureImporterType.Sprite
		};

		private readonly Dictionary<TextureImporterType, TextureImporterShape> textureShapeCaps = new Dictionary<TextureImporterType, TextureImporterShape>
		{
			{
				TextureImporterType.Default,
				TextureImporterShape.Texture2D | TextureImporterShape.TextureCube
			},
			{
				TextureImporterType.Sprite,
				TextureImporterShape.Texture2D
			}
		};

		private TextureImporterSettingsTemplate importerSettingsTemplate;
		private Styles styles;
		private bool showAdvanced;
		private bool showPlatformDefault;
		private bool showPerAxisWrapModes;


		#region TextureImporterSettings Properties
		private IPropertyLayout alphaSource;
		private IPropertyLayout mipMapMode;
		private IPropertyLayout enableMipMap;
		private IPropertyLayout fadeOut;
		private IPropertyLayout borderMipMap;
		private IPropertyLayout mipMapsPreserveCoverage;
		private IPropertyLayout alphaTestReferenceValue;
		private IPropertyLayout mipMapFadeDistance;
		private IPropertyLayout isReadable;
		private IPropertyLayout streamingMipmaps;
		private IPropertyLayout streamingMipmapsPriority;
		private IPropertyLayout NPOTScale;
		private IPropertyLayout sRGBTexture;
		private IPropertyLayout spriteMode;
		private IPropertyLayout spriteExtrude;
		private IPropertyLayout spriteMeshType;
		private IPropertyLayout spriteAlignment;
		private IPropertyLayout spritePivot;
		private IPropertyLayout spritePixelsToUnits;
		private IPropertyLayout spriteGenerateFallbackPhysicsShape;
		private IPropertyLayout alphaIsTransparency;
		private IPropertyLayout textureType;
		private IPropertyLayout textureShape;
		private IPropertyLayout filterMode;
		private IPropertyLayout aniso;

		private SerializedProperty wrapU;
		private SerializedProperty wrapV;
		private SerializedProperty wrapW;
		#endregion

		#region TextureImporterPlatformSettings
		private PlatformSettingsInspector platformDefault;
		private Dictionary<BuildTargetGroup, PlatformSettingsInspector> platformOverrides;
		#endregion


		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

			this.DrawImporterSettingsGUI();
			this.DrawImporterPlatformSettingsGUI();

			this.serializedObject.ApplyModifiedProperties();
		}


		private void OnEnable()
		{
			this.Initialize();
		}

		private void Initialize()
		{
			this.importerSettingsTemplate = this.target as TextureImporterSettingsTemplate;
			this.styles = this.styles ?? new Styles();

			this.showAdvanced = EditorPrefs.GetBool(TEXTURE_IMPORTER_SHOW_ADVANCED);
			this.showPlatformDefault = EditorPrefs.GetBool(TEXTURE_IMPORTER_PLATFORM_SHOW_DEFAULT);

			this.SetImporterSettingsPropertyCache();
			this.SetImporterPlatformSettingsPropertyCache();
		}

		private void SetImporterSettingsPropertyCache()
		{
			this.alphaSource = new PropertyLayoutIntPopup(
				"Alpha Source",
				this.serializedObject.FindProperty("importerSettings.m_AlphaSource"),
				this.styles.alphaSourceOptions,
				Enum.GetValues(typeof(TextureImporterAlphaSource)) as int[]
			);

			this.mipMapMode = new PropertyLayoutPopup(
				"Mip Map Filtering",
				this.serializedObject.FindProperty("importerSettings.m_MipMapMode"),
				this.styles.mipMapFilterOptions
			);

			this.enableMipMap = new PropertyLayoutIntToggle(
				"Generate Mip Maps",
				this.serializedObject.FindProperty("importerSettings.m_EnableMipMap")
			);

			this.fadeOut = new PropertyLayoutIntToggle(
				"Fadeout Mip Maps",
				this.serializedObject.FindProperty("importerSettings.m_FadeOut")
			);

			this.borderMipMap = new PropertyLayoutIntToggle(
				"Border Mip Maps",
				this.serializedObject.FindProperty("importerSettings.m_BorderMipMap")
			);

			this.mipMapsPreserveCoverage = new PropertyLayoutIntToggle(
				"Mip Maps Preserve Coverage",
				this.serializedObject.FindProperty("importerSettings.m_MipMapsPreserveCoverage")
			);

			this.alphaTestReferenceValue = new PropertyLayoutField(
				"Alpha Cutoff Value",
				this.serializedObject.FindProperty("importerSettings.m_AlphaTestReferenceValue")
			);

			this.mipMapFadeDistance = new PropertyLayoutMinMaxSlider(
				"Fade Range",
				this.serializedObject.FindProperty("importerSettings.m_MipMapFadeDistanceStart"),
				this.serializedObject.FindProperty("importerSettings.m_MipMapFadeDistanceEnd"),
				0,
				10,
				true
			);

			this.isReadable = new PropertyLayoutIntToggle(
				"Read/Write Enabled",
				this.serializedObject.FindProperty("importerSettings.m_IsReadable")
			);

			this.streamingMipmaps = new PropertyLayoutIntToggle(
				"Streaming Mipmaps",
				this.serializedObject.FindProperty("importerSettings.m_StreamingMipmaps")
			);

			this.streamingMipmapsPriority = new PropertyLayoutField(
				"Mip Map Priority",
				this.serializedObject.FindProperty("importerSettings.m_StreamingMipmapsPriority")
			);

			this.NPOTScale = new PropertyLayoutEnumPopup(
				"Non-Power of 2",
				this.serializedObject.FindProperty("importerSettings.m_NPOTScale"),
				typeof(TextureImporterNPOTScale)
			);

			this.sRGBTexture = new PropertyLayoutIntToggle(
				"sRGB (Color Texture)",
				this.serializedObject.FindProperty("importerSettings.m_sRGBTexture")
			);

			this.spriteMode = new PropertyLayoutIntPopup(
				"Sprite Mode",
				this.serializedObject.FindProperty("importerSettings.m_SpriteMode"),
				this.styles.spriteModeOptions,
				this.styles.spriteModeValues
			);

			this.spriteExtrude = new PropertyLayoutIntSlider(
				"Extrude Edges",
				this.serializedObject.FindProperty("importerSettings.m_SpriteExtrude"),
				0,
				32
			);

			this.spriteMeshType = new PropertyLayoutIntPopup(
				"Mesh Type",
				this.serializedObject.FindProperty("importerSettings.m_SpriteMeshType"),
				this.styles.spriteMeshTypeOptions,
				this.styles.spriteMeshTypeValues
			);

			this.spriteAlignment = new PropertyLayoutPopup(
				"Pivot",
				this.serializedObject.FindProperty("importerSettings.m_Alignment"),
				this.styles.spriteAlignmentOptions
			);

			this.spritePivot = new PropertyLayoutField(
				" ",
				this.serializedObject.FindProperty("importerSettings.m_SpritePivot")
			);

			this.spritePixelsToUnits = new PropertyLayoutField(
				"Pixels Per Unit",
				this.serializedObject.FindProperty("importerSettings.m_SpritePixelsToUnits")
			);

			this.spriteGenerateFallbackPhysicsShape = new PropertyLayoutIntToggle(
				"Generate Physics Shape",
				this.serializedObject.FindProperty("importerSettings.m_SpriteGenerateFallbackPhysicsShape")
			);

			this.alphaIsTransparency = new PropertyLayoutIntToggle(
				"Alpha Is Transparency",
				this.serializedObject.FindProperty("importerSettings.m_AlphaIsTransparency")
			);

			this.textureType = new PropertyLayoutIntPopup(
				"Texture Type",
				this.serializedObject.FindProperty("importerSettings.m_TextureType"),
				this.enabledTextureImporterTypes.Select(x => this.styles.textureTypeOptions[x]).ToArray(),
				this.enabledTextureImporterTypes.Select(x => (int)x).ToArray()
			);

			this.textureShape = new PropertyLayoutIntPopup(
				"Texture Shape",
				this.serializedObject.FindProperty("importerSettings.m_TextureShape"),
				Enum.GetNames(typeof(TextureImporterShape)),
				Enum.GetValues(typeof(TextureImporterShape)) as int[]
			);

			this.filterMode = new PropertyLayoutIntPopup(
				"Filter Mode",
				this.serializedObject.FindProperty("importerSettings.m_FilterMode"),
				this.styles.filterModeOptions,
				Enum.GetValues(typeof(FilterMode)) as int[]
			);

			this.aniso = new PropertyLayoutIntSlider(
				"Aniso Level",
				this.serializedObject.FindProperty("importerSettings.m_Aniso"),
				0,
				16
			);

			this.wrapU = this.serializedObject.FindProperty("importerSettings.m_WrapU");
			this.wrapV = this.serializedObject.FindProperty("importerSettings.m_WrapV");
			this.wrapW = this.serializedObject.FindProperty("importerSettings.m_WrapW");
		}

		private void SetImporterPlatformSettingsPropertyCache()
		{
			this.platformDefault = new PlatformSettingsInspector(
				this.serializedObject.FindProperty("defaultPlatform"),
				this.importerSettingsTemplate.DefaultPlatform,
				(TextureImporterType)this.textureType.Property.intValue,
				(SpriteImportMode)this.spriteMode.Property.intValue
			);

			this.platformOverrides = new Dictionary<BuildTargetGroup, PlatformSettingsInspector>();

			for (int i = 0; i < this.importerSettingsTemplate.PlatformGroups.Count; i++) {
				this.platformOverrides.Add(
					BuildPipeline.GetBuildTargetGroup(this.importerSettingsTemplate.PlatformGroups[i].Target),
					new PlatformSettingsInspector(
						this.serializedObject.FindProperty("platformGroups").GetArrayElementAtIndex(i),
						this.importerSettingsTemplate.PlatformGroups[i],
						(TextureImporterType)this.textureType.Property.intValue,
						(SpriteImportMode)this.spriteMode.Property.intValue
					)
				);
			}
		}


		private void ValidateProperties(TextureImporterType selectedTextureImporterType)
		{
			bool isUnfind = false;
			isUnfind = !Enum.IsDefined(typeof(TextureImporterShape), this.textureShape.Property.intValue);
			isUnfind = isUnfind || !this.textureShapeCaps[selectedTextureImporterType].HasFlag((TextureImporterShape)this.textureShape.Property.intValue);

			if (isUnfind) {
				Array enumValues = Enum.GetValues(typeof(TextureImporterShape));
				for (int i = 0; i < enumValues.Length; i++) {
					TextureImporterShape enumValue = (TextureImporterShape)enumValues.GetValue(i);
					if (this.textureShapeCaps[selectedTextureImporterType].HasFlag(enumValue)) {
						this.textureShape.Property.intValue = (int)enumValue;
						break;
					}
				}
			}
		}


		private void DrawImporterSettingsGUI()
		{
			EditorGUILayout.BeginVertical();
			EditorGUILayout.Space();

			this.textureType.Draw();
			TextureImporterType selectedTextureImporterType = (TextureImporterType)this.textureType.Property.intValue;

			this.ValidateProperties(selectedTextureImporterType);

			this.textureShape.SetValues(
				this.styles.textureShapeOptions[this.textureShapeCaps[selectedTextureImporterType]],
				this.styles.textureShapeValues[this.textureShapeCaps[selectedTextureImporterType]]
			);
			GUI.enabled = this.styles.textureShapeOptions[this.textureShapeCaps[selectedTextureImporterType]].Length > 1;
			this.textureShape.Draw();
			GUI.enabled = true;

			EditorGUILayout.Space();

			switch (selectedTextureImporterType) {
				case TextureImporterType.Default: this.DrawImporterSettingsTypeDefaultGUI(); break;
				case TextureImporterType.Sprite: this.DrawImporterSettingsTypeSpriteGUI(); break;
			}

			EditorPrefs.SetBool(TEXTURE_IMPORTER_SHOW_ADVANCED, this.showAdvanced);

			EditorGUILayout.EndVertical();
		}

		private void DrawImporterSettingsTypeDefaultGUI()
		{
			this.sRGBTexture.Draw();
			this.alphaSource.Draw();
			this.alphaIsTransparency.Draw();

			EditorGUILayout.Space();
			this.showAdvanced = EditorGUILayout.Foldout(this.showAdvanced, "Advanced");

			if (this.showAdvanced) {
				EditorGUI.indentLevel++;

				this.NPOTScale.Draw();
				this.isReadable.Draw();
				this.streamingMipmaps.Draw();

				if (this.streamingMipmaps.Property.intValue > 0) {
					EditorGUI.indentLevel++;
					this.streamingMipmapsPriority.Draw();
					EditorGUI.indentLevel--;
				}
				this.DrawImporterSettingsMipMapsGUI();

				EditorGUI.indentLevel--;
			}

			EditorGUILayout.Space();
			this.DrawImporterSettingsCommonGUI();
		}

		private void DrawImporterSettingsTypeSpriteGUI()
		{
			this.spriteMode.Draw();
			SpriteImportMode selectedSpriteMode = (SpriteImportMode)this.spriteMode.Property.intValue;

			EditorGUI.indentLevel++;

			switch (selectedSpriteMode) {
			case SpriteImportMode.Single:
				this.spritePixelsToUnits.Draw();
				this.spriteMeshType.Draw();
				this.spriteExtrude.Draw();
				this.spriteAlignment.Draw();
				if (this.spriteAlignment.Property.intValue == (int)SpriteAlignment.Custom) {
					this.spritePivot.Draw();
				}
				this.spriteGenerateFallbackPhysicsShape.Draw();
				break;

			case SpriteImportMode.Multiple:
				this.spritePixelsToUnits.Draw();
				this.spriteMeshType.Draw();
				this.spriteExtrude.Draw();
				this.spriteGenerateFallbackPhysicsShape.Draw();
				break;

			case SpriteImportMode.Polygon:
				this.spritePixelsToUnits.Draw();
				this.spriteExtrude.Draw();
				break;
			}

			EditorGUI.indentLevel--;

			EditorGUILayout.Space();
			this.showAdvanced = EditorGUILayout.Foldout(this.showAdvanced, "Advanced");

			if (this.showAdvanced) {
				this.sRGBTexture.Draw();
				this.alphaSource.Draw();
				this.alphaIsTransparency.Draw();
				this.DrawImporterSettingsMipMapsGUI();
			}

			EditorGUILayout.Space();
			this.DrawImporterSettingsCommonGUI();
		}

		private void DrawImporterSettingsMipMapsGUI()
		{
			this.enableMipMap.Draw();

			if (this.enableMipMap.Property.intValue > 0) {
				EditorGUI.indentLevel++;
				this.borderMipMap.Draw();
				this.mipMapMode.Draw();
				this.mipMapsPreserveCoverage.Draw();

				if (this.mipMapsPreserveCoverage.Property.intValue > 0) {
					EditorGUI.indentLevel++;
					this.alphaTestReferenceValue.Draw();
					EditorGUI.indentLevel--;
				}
				this.fadeOut.Draw();

				if (this.fadeOut.Property.intValue > 0) {
					EditorGUI.indentLevel++;
					this.mipMapFadeDistance.Draw();
					EditorGUI.indentLevel--;
				}
				EditorGUI.indentLevel--;
			}
		}

		private void DrawImporterSettingsCommonGUI()
		{
			InternalsAccess.WrapModePopup(this.wrapU, this.wrapV, this.wrapW, false, ref this.showPerAxisWrapModes);
			this.filterMode.Draw();

			bool showAniso = true;
			showAniso &= (FilterMode)this.filterMode.Property.intValue != FilterMode.Point;
			showAniso &= this.enableMipMap.Property.intValue > 0;
			showAniso &= (TextureImporterShape)this.textureShape.Property.intValue != TextureImporterShape.TextureCube;

			GUI.enabled = showAniso;
			this.aniso.Draw();
			GUI.enabled = true;
		}

		private void DrawImporterPlatformSettingsGUI()
		{
			TextureImporterType selectedTextureImporterType = (TextureImporterType)this.textureType.Property.intValue;
			EditorGUILayout.Space();

			this.showPlatformDefault = EditorGUILayout.Foldout(this.showPlatformDefault, "Default");

			if (this.showPlatformDefault) {
				EditorGUILayout.BeginVertical(GUI.skin.box);
				this.platformDefault.Draw();
				EditorGUILayout.EndVertical();
			}
			EditorPrefs.SetBool(TEXTURE_IMPORTER_PLATFORM_SHOW_DEFAULT, this.showPlatformDefault);

			EditorGUILayout.Space();

			BuildTargetGroup buildTargetGroup = EditorGUILayout.BeginBuildTargetSelectionGrouping();

			if (!this.platformOverrides.ContainsKey(buildTargetGroup)) {
				throw new NotSupportedException($"{buildTargetGroup} is unknown platform.");
			}
			this.platformOverrides[buildTargetGroup].SetFormatProperty(selectedTextureImporterType);
			this.platformOverrides[buildTargetGroup].Draw();

			EditorGUILayout.EndBuildTargetSelectionGrouping();
		}


		private class Styles
		{
			public readonly Dictionary<TextureImporterType, string> textureTypeOptions = new Dictionary<TextureImporterType, string>
			{
				{ TextureImporterType.Default, "Default" },
				{ TextureImporterType.Sprite, "Sprite (2D and UI)" }
			};

			public readonly Dictionary<TextureImporterShape, string[]> textureShapeOptions = new Dictionary<TextureImporterShape, string[]>
			{
				{ TextureImporterShape.Texture2D, new string[] { "2D" } },
				{ TextureImporterShape.TextureCube, new string[] { "Cube" } },
				{ TextureImporterShape.Texture2D | TextureImporterShape.TextureCube, new string[] { "2D", "Cube" } }
			};
			public readonly Dictionary<TextureImporterShape, int[]> textureShapeValues = new Dictionary<TextureImporterShape, int[]>
			{
				{ TextureImporterShape.Texture2D, new int[] { (int)TextureImporterShape.Texture2D } },
				{ TextureImporterShape.TextureCube, new int[] { (int)TextureImporterShape.TextureCube } },
				{ TextureImporterShape.Texture2D | TextureImporterShape.TextureCube, new int[] { (int)TextureImporterShape.Texture2D, (int)TextureImporterShape.TextureCube } }
			};

			public readonly string[] alphaSourceOptions =
			{
				"None",
				"Input Texture Alpha",
				"From Gray Scale"
			};

			public readonly string[] spriteModeOptions =
			{
				"Single",
				"Multiple",
				"Polygon"
			};
			public readonly int[] spriteModeValues =
			{
				(int)SpriteImportMode.Single,
				(int)SpriteImportMode.Multiple,
				(int)SpriteImportMode.Polygon
			};

			public readonly string[] spriteMeshTypeOptions =
			{
				"Full Rect",
				"Tight"
			};
			public readonly int[] spriteMeshTypeValues =
			{
				(int)SpriteMeshType.FullRect,
				(int)SpriteMeshType.Tight
			};

			public readonly string[] spriteAlignmentOptions =
			{
				"Center",
				"Top Left",
				"Top",
				"Top Right",
				"Left",
				"Right",
				"Bottom Left",
				"Bottom",
				"Bottom Right",
				"Custom"
			};

			public readonly string[] mipMapFilterOptions =
			{
				"Box",
				"Kaiser"
			};

			public readonly string[] filterModeOptions =
			{
				"Point (no filter)",
				"Bilinear",
				"Trilinear"
			};
		}


		private class PlatformSettingsInspector
		{
			private readonly int[] maxTextureSizeValues = { 32, 64, 128, 256, 512, 1024, 2048, 4096, 8192 };

			private readonly string[] textureCompressionOptions =
			{
				"None",
				"Low Quality",
				"Normal Quality",
				"High Quality"
			};
			private readonly int[] textureCompressionValues =
			{
				(int)TextureImporterCompression.Uncompressed,
				(int)TextureImporterCompression.CompressedLQ,
				(int)TextureImporterCompression.Compressed,
				(int)TextureImporterCompression.CompressedHQ
			};

			private readonly string[] androidETC2FallbackOptions =
			{
				"Use build settings",
				"32-bit",
				"16-bit",
				"32-bit (half resolution)"
			};
			private readonly int[] androidETC2FallbackValues =
			{
				(int)AndroidETC2FallbackOverride.UseBuildSettings,
				(int)AndroidETC2FallbackOverride.Quality32Bit,
				(int)AndroidETC2FallbackOverride.Quality16Bit,
				(int)AndroidETC2FallbackOverride.Quality32BitDownscaled
			};

			private readonly SerializedProperty serializedProperty;
			private readonly TextureImporterPlatformSettingsGroup platformSettingsGroup;
			private readonly TextureImporterType selectedTextureImporterType;
			private readonly SpriteImportMode selectedSpriteImportMode;

			private IPropertyLayout overridden;
			private IPropertyLayout maxSize;
			private IPropertyLayout resizeAlgorithm;
			private IPropertyLayout format;
			private IPropertyLayout compresssion;
			private IPropertyLayout crunchedCompression;
			private IPropertyLayout compressionQualityPopup;
			private IPropertyLayout compressionQualitySlider;
			private IPropertyLayout allowsAlphaSplitting;
			private IPropertyLayout androidETC2FallbackOverride;

			public PlatformSettingsInspector(SerializedProperty serializedProperty, TextureImporterPlatformSettingsGroup platformSettingsGroup, TextureImporterType textureType, SpriteImportMode spriteMode)
			{
				this.serializedProperty = serializedProperty;
				this.platformSettingsGroup = platformSettingsGroup;
				this.selectedTextureImporterType = textureType;
				this.selectedSpriteImportMode = spriteMode;

				bool isStandalone = BuildPipeline.GetBuildTargetGroup(this.platformSettingsGroup.Target) == BuildTargetGroup.Standalone;

				this.overridden = new PropertyLayoutToggleLeft(
					isStandalone ? "Override for PC, Mac & Linux Standalone" : $"Override for {this.platformSettingsGroup.Target}",
					this.serializedProperty.FindPropertyRelative("settings.m_Overridden")
				);

				this.maxSize = new PropertyLayoutIntPopup(
					"Max Size",
					this.serializedProperty.FindPropertyRelative("settings.m_MaxTextureSize"),
					this.maxTextureSizeValues
				);

				this.resizeAlgorithm = new PropertyLayoutEnumPopup(
					"Resize Algorithm",
					this.serializedProperty.FindPropertyRelative("settings.m_ResizeAlgorithm"),
					typeof(TextureResizeAlgorithm)
				);

				this.SetFormatProperty(this.selectedTextureImporterType);

				this.compresssion = new PropertyLayoutIntPopup(
					"Compression",
					this.serializedProperty.FindPropertyRelative("settings.m_TextureCompression"),
					this.textureCompressionOptions,
					this.textureCompressionValues
				);

				this.crunchedCompression = new PropertyLayoutIntToggle(
					"Use Crunch Compression",
					this.serializedProperty.FindPropertyRelative("settings.m_CrunchedCompression")
				);

				this.compressionQualityPopup = new PropertyLayoutEnumPopup(
					"Compression Quality",
					this.serializedProperty.FindPropertyRelative("settings.m_CompressionQuality"),
					typeof(TextureCompressionQuality)
				);

				this.compressionQualitySlider = new PropertyLayoutIntSlider(
					"Compression Quality",
					this.serializedProperty.FindPropertyRelative("settings.m_CompressionQuality"),
					0,
					100
				);

				this.allowsAlphaSplitting = new PropertyLayoutToggle(
					"Split Alpha Channel",
					this.serializedProperty.FindPropertyRelative("settings.m_AllowsAlphaSplitting")
				);

				if (this.platformSettingsGroup.Target == BuildTarget.Android) {
					this.androidETC2FallbackOverride = new PropertyLayoutIntPopup(
						"Override ETC2 fallback",
						this.serializedProperty.FindPropertyRelative("settings.m_AndroidETC2FallbackOverride"),
						this.androidETC2FallbackOptions,
						this.androidETC2FallbackValues
					);
				}
			}

			public void SetFormatProperty(TextureImporterType textureImporterType)
			{
				Tuple<int[], string[]> formatValuesAndStrings;

				if (this.platformSettingsGroup.IsDefault) {
					formatValuesAndStrings = InternalsAccess.GetDefaultTextureFormatValuesAndStrings(textureImporterType);
				}
				else {
					formatValuesAndStrings = InternalsAccess.GetPlatformTextureFormatValuesAndStrings(textureImporterType, this.platformSettingsGroup.Target);
				}

				if (this.format == null) {
					this.format = new PropertyLayoutIntPopup(
						"Format",
						this.serializedProperty.FindPropertyRelative("settings.m_TextureFormat"),
						formatValuesAndStrings.Item2,
						formatValuesAndStrings.Item1
					);
				}
				else {
					this.format.SetValues(formatValuesAndStrings.Item2, formatValuesAndStrings.Item1);
				}
			}

			public void Draw()
			{
				if (!this.platformSettingsGroup.IsDefault) {
					this.overridden.Draw();
				}
				GUI.enabled = this.platformSettingsGroup.IsDefault || this.overridden.Property.boolValue;

				this.maxSize.Draw();
				this.resizeAlgorithm.Draw();
				this.format.Draw();

				TextureImporterFormat selectedImportFormat = (TextureImporterFormat)this.format.Property.intValue;

				if (this.platformSettingsGroup.IsDefault && selectedImportFormat == TextureImporterFormat.Automatic) {
					this.compresssion.Draw();
				}
				TextureImporterCompression selectedImportCompression = (TextureImporterCompression)this.compresssion.Property.intValue;

				if (this.platformSettingsGroup.IsDefault) {
					if (selectedImportFormat == TextureImporterFormat.Automatic && selectedImportCompression != TextureImporterCompression.Uncompressed) {
						this.crunchedCompression.Draw();
					}
					if (this.crunchedCompression.Property.intValue > 0) {
						this.compressionQualitySlider.Draw();
					}
				}
				else {
					bool isCrunchedFormat = InternalsAccess.IsCompressedCrunchTextureFormat((TextureFormat)(int)selectedImportFormat);

					if (isCrunchedFormat || this.IsCompressionTarget(selectedImportFormat)) {
						this.DrawPlatformCompressionQuality(this.platformSettingsGroup.Target, isCrunchedFormat, selectedImportFormat);
					}

					bool isETCPlatform = InternalsAccess.IsETC1SupportedByBuildTarget(this.platformSettingsGroup.Target);
					bool isDealingWithSprite = this.selectedSpriteImportMode != SpriteImportMode.None;
					bool isETCFormatSelected = InternalsAccess.IsTextureFormatETC1Compression((TextureFormat)(int)selectedImportFormat);

					if (isETCPlatform && isDealingWithSprite && isETCFormatSelected) {
						this.allowsAlphaSplitting.Draw();
					}

					if (this.platformSettingsGroup.Target == BuildTarget.Android) {
						this.androidETC2FallbackOverride.Draw();
					}
				}

				GUI.enabled = true;
			}

			private void DrawPlatformCompressionQuality(BuildTarget target, bool isCrunchedFormat, TextureImporterFormat textureFormat)
			{
				bool showAsEnum = !isCrunchedFormat
					&& (InternalsAccess.PlatformHasIntegratedGPU(target) || (textureFormat == TextureImporterFormat.BC6H) || (textureFormat == TextureImporterFormat.BC7));

				if (showAsEnum) {
					this.compressionQualityPopup.Draw();
				}
				else {
					this.compressionQualitySlider.Draw();
				}
			}

			private bool IsCompressionTarget(TextureImporterFormat textureImporterFormat)
			{
				TextureImporterFormat[] compressionTargetFormats = InternalsAccess.GetCompressionTextureImporterFormats();
				return compressionTargetFormats.ToList().Contains(textureImporterFormat);
			}
		}


		private static class InternalsAccess
		{
			public static void WrapModePopup(SerializedProperty wrapU, SerializedProperty wrapV, SerializedProperty wrapW, bool isVolumeTexture, ref bool showPerAxisWrapModes)
			{
				object[] args = { wrapU, wrapV, wrapW, isVolumeTexture, null };
				Type.GetType("UnityEditor.TextureInspector, UnityEditor")
					.GetMethod("WrapModePopup", BindingFlags.Static | BindingFlags.NonPublic)
					.Invoke(null, args);

				showPerAxisWrapModes = (bool)args[4];
			}

			public static Tuple<int[], string[]> GetDefaultTextureFormatValuesAndStrings(TextureImporterType textureType)
			{
				object[] args = { textureType, null, null };
				Type.GetType("UnityEditor.TextureImportValidFormats, UnityEditor")
					.GetMethod("GetDefaultTextureFormatValuesAndStrings", BindingFlags.Static | BindingFlags.Public)
					.Invoke(null, args);

				return new Tuple<int[], string[]>(args[1] as int[], args[2] as string[]);
			}

			public static Tuple<int[], string[]> GetPlatformTextureFormatValuesAndStrings(TextureImporterType textureType, BuildTarget target)
			{
				object[] args = { textureType, target, null, null };
				Type.GetType("UnityEditor.TextureImportValidFormats, UnityEditor")
					.GetMethod("GetPlatformTextureFormatValuesAndStrings", BindingFlags.Static | BindingFlags.Public)
					.Invoke(null, args);

				return new Tuple<int[], string[]>(args[2] as int[], args[3] as string[]);
			}

			public static bool IsCompressedCrunchTextureFormat(TextureFormat format)
			{
				return (bool)Type.GetType("UnityEditor.TextureUtil, UnityEditor")
					.GetMethod("IsCompressedCrunchTextureFormat", BindingFlags.Static | BindingFlags.Public)
					.Invoke(null, new object[] { format });
			}

			public static TextureImporterFormat[] GetCompressionTextureImporterFormats()
			{
				return (TextureImporterFormat[])Type.GetType("UnityEditor.TextureImporterInspector, UnityEditor")
					.GetField("kFormatsWithCompressionSettings", BindingFlags.Static | BindingFlags.NonPublic)
					.GetValue(null);
			}

			public static bool PlatformHasIntegratedGPU(BuildTarget target)
			{
				Type targetAttributesEnum = Type.GetType("UnityEditor.BuildTargetDiscovery+TargetAttributes, UnityEditor");
				int targetEnumIndex = Enum.GetNames(targetAttributesEnum).ToList().IndexOf("HasIntegratedGPU");
				object targetAttributesEnumType = Enum.GetValues(targetAttributesEnum).GetValue(targetEnumIndex);

				return (bool)Type.GetType("UnityEditor.BuildTargetDiscovery, UnityEditor")
					.GetMethod("PlatformHasFlag", BindingFlags.Static | BindingFlags.Public)
					.Invoke(null, new object[] { target, targetAttributesEnumType });
			}

			public static bool IsETC1SupportedByBuildTarget(BuildTarget target)
			{
				return (bool)Type.GetType("UnityEditor.TextureImporter, UnityEditor")
					.GetMethod("IsETC1SupportedByBuildTarget", BindingFlags.Static | BindingFlags.NonPublic)
					.Invoke(null, new object[] { target });
			}

			public static bool IsTextureFormatETC1Compression(TextureFormat format)
			{
				return (bool)Type.GetType("UnityEditor.TextureImporter, UnityEditor")
					.GetMethod("IsTextureFormatETC1Compression", BindingFlags.Static | BindingFlags.NonPublic)
					.Invoke(null, new object[] { format });
			}
		}
	}
}