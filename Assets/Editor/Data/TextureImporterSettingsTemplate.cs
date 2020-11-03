﻿using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

namespace Unitils
{
	public class TextureImporterSettingsTemplate : ScriptableObject
	{
		[SerializeField]
		private TextureImporterSettings importerSettings;
		public TextureImporterSettings ImporterSettings => this.importerSettings;

		[SerializeField]
		private TextureImporterPlatformSettingsGroup defaultPlatform;
		public TextureImporterPlatformSettingsGroup DefaultPlatform => this.defaultPlatform;

		[SerializeField]
		private List<TextureImporterPlatformSettingsGroup> platformGroups;
		public IReadOnlyList<TextureImporterPlatformSettingsGroup> PlatformGroups => this.platformGroups;


		public TextureImporterSettingsTemplate()
		{
			#region TextureImporterSettings
			this.importerSettings = new TextureImporterSettings
			{
				sRGBTexture = true,
				alphaSource = TextureImporterAlphaSource.FromInput,
				alphaIsTransparency = false,

				spriteMode = (int)SpriteImportMode.Single,
				spritePixelsPerUnit = 100,
				spriteMeshType = SpriteMeshType.Tight,
				spriteExtrude = 1u,
				spriteAlignment = (int)SpriteAlignment.Center,
				spritePivot = new Vector2(0.5f, 0.5f),
				spriteGenerateFallbackPhysicsShape = true,

				npotScale = TextureImporterNPOTScale.None,
				readable = false,
				streamingMipmaps = false,
				mipmapEnabled = false,
				borderMipmap = false,
				mipmapFilter = TextureImporterMipFilter.BoxFilter,
				mipMapsPreserveCoverage = false,
				alphaTestReferenceValue = 0.5f,
				fadeOut = false,
				mipmapFadeDistanceStart = 1,
				mipmapFadeDistanceEnd = 3,

				wrapMode = TextureWrapMode.Repeat,
				filterMode = FilterMode.Bilinear,
				aniso = 1
			};
			#endregion

			#region TextureImporterPlatformSettings
			this.defaultPlatform = new TextureImporterPlatformSettingsGroup();

			BuildTarget targetStandalone;
			switch (Application.platform) {
				case RuntimePlatform.OSXEditor:
					targetStandalone = BuildTarget.StandaloneOSX;
					break;
				case RuntimePlatform.WindowsEditor:
					targetStandalone = BuildTarget.StandaloneWindows;
					break;
				case RuntimePlatform.LinuxEditor:
					targetStandalone = BuildTarget.StandaloneLinux64;
					break;
				default:
					throw new NotSupportedException($"{Application.platform} is unknown platform.");
			}

			this.platformGroups = new List<TextureImporterPlatformSettingsGroup>
			{
				new TextureImporterPlatformSettingsGroup(
					targetStandalone,
					new TextureImporterPlatformSettings
					{
						format = TextureImporterFormat.DXT1
					}
				),
				new TextureImporterPlatformSettingsGroup(
					BuildTarget.iOS,
					new TextureImporterPlatformSettings
					{
						format = TextureImporterFormat.PVRTC_RGB4
					}
				),
				new TextureImporterPlatformSettingsGroup(
					BuildTarget.Android,
					new TextureImporterPlatformSettings
					{
						format = TextureImporterFormat.ETC_RGB4
					}
				)
			};
			#endregion
		}
	}
}