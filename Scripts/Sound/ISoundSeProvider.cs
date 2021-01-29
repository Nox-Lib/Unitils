using System.Collections.Generic;

namespace Unitils
{
	public interface ISoundSeProvider
	{
		float Volume { get; set; }
		float Pan { get; set; }

		void Load(string key, int group = 0);
		void Load(IEnumerable<string> keys, int group = 0);
		void Unload(int group = 0);
		void Play(string key, float volumeScale = 1f);
		void Stop();
		float GetLength(string key);
	}
}