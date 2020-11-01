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

		private class TextureImporterTypeData
		{
			public TextureImporterType TextureType { get; }
			public TextureImporterShape TextureShape { get; }

			public TextureImporterTypeData(TextureImporterType textureType, TextureImporterShape textureShape)
			{
				this.TextureType = textureType;
				this.TextureShape = textureShape;
			}
		}

		private TextureImporterTypeData[] enabledTextureImporterTypes =
		{
			new TextureImporterTypeData(
				TextureImporterType.Default,
				TextureImporterShape.Texture2D | TextureImporterShape.TextureCube
			),
			new TextureImporterTypeData(
				TextureImporterType.Sprite,
				TextureImporterShape.Texture2D
			)
		};

		private TextureImporterSettingsTemplate importerSettingsTemplate;
		private TextureImporterSettings importerSettings;
		private Styles styles;
		private bool showAdvanced;
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


		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

			this.DrawTextureImporterGUI();

			this.serializedObject.ApplyModifiedProperties();
		}


		private void OnEnable()
		{
			this.Initialize();
		}

		private void Initialize()
		{
			this.importerSettingsTemplate = this.target as TextureImporterSettingsTemplate;
			this.importerSettings = this.importerSettingsTemplate.ImporterSettings;
			this.styles = this.styles ?? new Styles();

			this.InitializePropertyCache();
		}

		private void InitializePropertyCache()
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
				enabledTextureImporterTypes.Select(x => this.styles.textureTypeOptions[x.TextureType]).ToArray(),
				enabledTextureImporterTypes.Select(x => (int)x.TextureType).ToArray()
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


		private void ValidateProperties(TextureImporterTypeData selectedTextureTypeData)
		{
			bool isUnfind = false;
			isUnfind = !Enum.IsDefined(typeof(TextureImporterShape), this.textureShape.Property.intValue);
			isUnfind = isUnfind || !selectedTextureTypeData.TextureShape.HasFlag((TextureImporterShape)this.textureShape.Property.intValue);

			if (isUnfind) {
				Array enumValues = Enum.GetValues(typeof(TextureImporterShape));
				for (int i = 0; i < enumValues.Length; i++) {
					TextureImporterShape enumValue = (TextureImporterShape)enumValues.GetValue(i);
					if (selectedTextureTypeData.TextureShape.HasFlag(enumValue)) {
						this.importerSettings.textureShape = enumValue;
						break;
					}
				}
			}
		}


		private void DrawTextureImporterGUI()
		{
			EditorGUILayout.BeginVertical();
			EditorGUILayout.Space();

			this.showAdvanced = EditorPrefs.GetBool(TEXTURE_IMPORTER_SHOW_ADVANCED);

			this.textureType.Draw();
			TextureImporterTypeData selectedTextureTypeData = enabledTextureImporterTypes
				.FirstOrDefault(x => x.TextureType == (TextureImporterType)this.textureType.Property.intValue);

			this.ValidateProperties(selectedTextureTypeData);

			this.textureShape.SetValues(
				this.styles.textureShapeOptions[selectedTextureTypeData.TextureShape],
				this.styles.textureShapeValues[selectedTextureTypeData.TextureShape]
			);
			GUI.enabled = this.styles.textureShapeOptions[selectedTextureTypeData.TextureShape].Length > 1;
			this.textureShape.Draw();
			GUI.enabled = true;

			EditorGUILayout.Space();

			switch (selectedTextureTypeData.TextureType) {
				case TextureImporterType.Default: this.DrawTextureImporterDefaultGUI(); break;
				case TextureImporterType.Sprite: this.DrawTextureImporterSpriteGUI(); break;
			}

			EditorPrefs.SetBool(TEXTURE_IMPORTER_SHOW_ADVANCED, this.showAdvanced);

			EditorGUILayout.EndVertical();
		}

		private void DrawTextureImporterDefaultGUI()
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
				this.DrawTextureImporterMipMapsGUI();

				EditorGUI.indentLevel--;
			}

			EditorGUILayout.Space();
			this.DrawTextureImporterCommonGUI();
		}

		private void DrawTextureImporterSpriteGUI()
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
				this.DrawTextureImporterMipMapsGUI();
			}

			EditorGUILayout.Space();
			this.DrawTextureImporterCommonGUI();
		}

		private void DrawTextureImporterMipMapsGUI()
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

		private void DrawTextureImporterCommonGUI()
		{
			WrapModePopup(this.wrapU, this.wrapV, this.wrapW, false, ref this.showPerAxisWrapModes);
			this.filterMode.Draw();

			bool showAniso = true;
			showAniso &= (FilterMode)this.filterMode.Property.intValue != FilterMode.Point;
			showAniso &= this.enableMipMap.Property.intValue > 0;
			showAniso &= (TextureImporterShape)this.textureShape.Property.intValue != TextureImporterShape.TextureCube;

			GUI.enabled = showAniso;
			this.aniso.Draw();
			GUI.enabled = true;
		}


		private static void WrapModePopup(SerializedProperty wrapU, SerializedProperty wrapV, SerializedProperty wrapW, bool isVolumeTexture, ref bool showPerAxisWrapModes)
		{
			object[] args = { wrapU, wrapV, wrapW, isVolumeTexture, showPerAxisWrapModes };
			Type.GetType("UnityEditor.TextureInspector, UnityEditor").GetMethod("WrapModePopup", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, args);
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
	}
}