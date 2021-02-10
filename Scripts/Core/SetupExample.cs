using System.Linq;

namespace Unitils
{
	public static class SetupExample
	{
		public static void Launch()
		{
			SceneTransitioner.Activation();

			AudioBgmPlayer.Activation();
			AudioSePlayer.Activation();
			AudioVoicePlayer.Activation();

			ISoundSeProvider soundSePlayer = ServiceLocator.Instance.GetService<ISoundSeProvider>();
			ButtonSoundData.Instance.GetSoundNames().ToList().ForEach(_ => soundSePlayer.Load(_));
		}
	}
}