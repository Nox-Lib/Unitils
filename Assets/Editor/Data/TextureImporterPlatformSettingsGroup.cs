using UnityEngine;
using UnityEditor;
using System;

namespace Unitils
{
	[Serializable]
	public class TextureImporterPlatformSettingsGroup
	{
		[SerializeField]
		private bool isDefault = false;
		public bool IsDefault => this.isDefault;

		[SerializeField]
		private BuildTarget target = default;
		public BuildTarget Target => this.target;

		#pragma warning disable 414
		[SerializeField]
		private bool isOverride = false;
		#pragma warning restore 414

		[SerializeField]
		private TextureImporterPlatformSettings settings;
		public TextureImporterPlatformSettings Settings => settings;


		public TextureImporterPlatformSettingsGroup()
		{
			this.isDefault = true;
			this.settings = new TextureImporterPlatformSettings();
		}

		public TextureImporterPlatformSettingsGroup(BuildTarget buildTarget, TextureImporterPlatformSettings platformSettings)
		{
			this.target = buildTarget;
			this.settings = platformSettings;
		}
	}
}