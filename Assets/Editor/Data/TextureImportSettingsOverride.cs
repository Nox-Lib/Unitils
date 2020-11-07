using UnityEngine;
using System;
using System.Collections.Generic;

namespace Unitils
{
	public class TextureImportSettingsOverride : ScriptableObject
	{
		private static TextureImportSettingsOverride instance = null;
		public static TextureImportSettingsOverride Instance {
			get {
				return instance = instance ?? Resources.Load<TextureImportSettingsOverride>(DefineData.TEXTURE_IMPORT_SETTINGS_OVERRIDE);
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

			[SerializeField] private TextureImportSettingsTemplate template;
			public TextureImportSettingsTemplate Template => this.template;

			[SerializeField] private string pathMatchPattern;
			public string PathMatchPattern => this.pathMatchPattern;

			[SerializeField] private string pathIgnorePattern;
			public string PathIgnorePattern => this.pathIgnorePattern;
		}

		[SerializeField] private List<Configuration> configurations;
		public List<Configuration> Configurations => this.configurations;
	}
}