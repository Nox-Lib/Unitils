using System.Collections.Generic;

namespace Unitils
{
	public interface ISoundVoiceProvider
	{
		bool IsPlaying { get; }
		float Volume { get; set; }
		float Pan { get; set; }
		float Time { get; set; }

		void Load(string key, int group = 0);
		void Load(IEnumerable<string> keys, int group = 0);
		void Unload(int group = 0);
		void Play(string key);
		void Stop();
		void Pause();
		float GetLength(string key);
	}
}