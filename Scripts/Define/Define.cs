namespace Unitils
{
	public static partial class Define
	{
		public const string SYSTEM_DATA = "Data/SystemData";
		public const string BUTTON_SOUND_DATA = "Data/ButtonSoundData";
		public const string TEXTURE_IMPORT_SETTINGS_OVERRIDE = "Data/OverrideTextureImportSettings/Configurations";

		public const string SAMPLE_SE = "Sounds/sample";

		public enum ScreenMode
		{
			Fixed = 0,
			Expand
		}

		public enum EncryptionType
		{
			None = 0,
			AES
		}

		public enum ButtonTrigger
		{
			None = 0,
			Click,
			Down,
			Up,
			Enter,
			Exit,
			DragStart,
			Drag,
			DragEnd
		}

		public enum ButtonSoundType
		{
			None = 0,
			Done,
			Cancel,
			Close,
			Select,
			Plus,
			Minus,
			Swipe,
			Scroll
		}
	}
}