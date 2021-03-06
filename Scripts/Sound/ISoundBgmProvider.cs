using System.Collections.Generic;

namespace Unitils
{
	public interface ISoundBgmProvider
	{
		bool IsPlaying { get; }
		float Volume { get; set; }
		float Pan { get; set; }
		float FadeTime { get; set; }
		float Time { get; set; }

		void Load(string key, int group = 0);
		void Load(IEnumerable<string> keys, int group = 0);
		void Unload(int group = 0);
		void Play(string key, bool isLoop);
		void Stop();
		void Pause();
		void UnPause();
		float GetLength(string key);
	}
}