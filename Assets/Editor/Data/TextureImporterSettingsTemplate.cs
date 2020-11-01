using UnityEngine;
using UnityEditor;

namespace Unitils
{
	public class TextureImporterSettingsTemplate : ScriptableObject
	{
		[SerializeField] private TextureImporterSettings importerSettings = new TextureImporterSettings();
		public TextureImporterSettings ImporterSettings => this.importerSettings;

		public void Initialize()
		{
			this.ImporterSettings.sRGBTexture = true;
			this.ImporterSettings.alphaSource = TextureImporterAlphaSource.FromInput;
			this.ImporterSettings.alphaIsTransparency = false;

			this.ImporterSettings.spriteMode = (int)SpriteImportMode.Single;
			this.ImporterSettings.spritePixelsPerUnit = 100;
			this.ImporterSettings.spriteMeshType = SpriteMeshType.Tight;
			this.ImporterSettings.spriteExtrude = 1u;
			this.ImporterSettings.spriteAlignment = (int)SpriteAlignment.Center;
			this.ImporterSettings.spritePivot = new Vector2(0.5f, 0.5f);
			this.ImporterSettings.spriteGenerateFallbackPhysicsShape = true;

			this.ImporterSettings.npotScale = TextureImporterNPOTScale.None;
			this.ImporterSettings.readable = false;
			this.ImporterSettings.streamingMipmaps = false;
			this.ImporterSettings.mipmapEnabled = false;
			this.ImporterSettings.borderMipmap = false;
			this.ImporterSettings.mipmapFilter = TextureImporterMipFilter.BoxFilter;
			this.ImporterSettings.mipMapsPreserveCoverage = false;
			this.ImporterSettings.alphaTestReferenceValue = 0.5f;
			this.ImporterSettings.fadeOut = false;
			this.ImporterSettings.mipmapFadeDistanceStart = 1;
			this.ImporterSettings.mipmapFadeDistanceEnd = 3;

			this.ImporterSettings.wrapMode = TextureWrapMode.Repeat;
			this.ImporterSettings.filterMode = FilterMode.Bilinear;
			this.ImporterSettings.aniso = 1;
		}
	}
}