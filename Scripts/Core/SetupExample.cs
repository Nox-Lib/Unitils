using System.Collections.Generic;

namespace Unitils
{
	public static class SetupExample
	{
		public static void Launch()
		{
			AudioBgmPlayer.Activation();
			AudioSePlayer.Activation();
			AudioVoicePlayer.Activation();

			ISoundSeProvider soundSePlayer = ServiceLocator.Instance.GetService<ISoundSeProvider>();
			IEnumerable<string> buttonSoundNames = ButtonSoundData.Instance.GetSoundNames();
			foreach (string soundName in buttonSoundNames) {
				soundSePlayer.Load(soundName);
			}
		}
	}
}