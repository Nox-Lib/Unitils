using System.Collections.Generic;
using UnityEngine;

namespace Unitils
{
	public class AudioSePlayer : MonoBehaviour, ISoundSeProvider
	{
		private static AudioSePlayer instance = null;

		public static void Activation()
		{
			if (instance == null) {
				instance = Utils.Unity.CreateGameObject<AudioSePlayer>("AudioSePlayer");
				DontDestroyOnLoad(instance.gameObject);
				instance.audioSource = instance.gameObject.AddComponent<AudioSource>();
				instance.audioSource.playOnAwake = false;
			}
			ServiceLocator.Instance.Register<ISoundSeProvider>(instance);
		}


		private class Cache
		{
			public List<int> groups = new List<int>();
			public AudioClip audioClip;
			public float lastPlayedTime;
		}

		private readonly Dictionary<string, Cache> caches = new Dictionary<string, Cache>();

		private AudioSource audioSource;


		#region ISoundSeProvider

		public float Volume {
			get { return this.audioSource.volume; }
			set { this.audioSource.volume = Mathf.Clamp01(value); }
		}

		public float Pan {
			get { return this.audioSource.panStereo; }
			set { this.audioSource.panStereo = Mathf.Clamp(value, -1f, 1f); }
		}

		public void Load(string key, int group = 0)
		{
			Cache cache;
			if (this.caches.ContainsKey(key)) {
				cache = this.caches[key];
			}
			else {
				cache = new Cache();
				this.caches.Add(key, cache);
			}

			if (!cache.groups.Contains(group)) {
				cache.groups.Add(group);
			}
			if (cache.audioClip == null) {
				cache.audioClip = Resources.Load<AudioClip>(key);
			}
		}

		public void Load(IEnumerable<string> keys, int group = 0)
		{
			foreach (string key in keys) {
				this.Load(key, group);
			}
		}

		public void Unload(int group = 0)
		{
			List<string> removeKeys = new List<string>();

			foreach (KeyValuePair<string, Cache> kvp in this.caches) {
				if (!kvp.Value.groups.Contains(group)) continue;

				kvp.Value.groups.Remove(group);
				if (kvp.Value.groups.Count >= 1) continue;

				if (kvp.Value.audioClip != null) {
					Resources.UnloadAsset(kvp.Value.audioClip);
					kvp.Value.audioClip = null;
				}
				removeKeys.Add(kvp.Key);
			}
			removeKeys.ForEach(key => this.caches.Remove(key));
		}

		public void Play(string key, float volumeScale = 1f)
		{
			if (!this.caches.ContainsKey(key) || this.caches[key].audioClip == null) return;
			if (Time.realtimeSinceStartup - this.caches[key].lastPlayedTime < 0.05f) return;
			this.audioSource.PlayOneShot(this.caches[key].audioClip, Mathf.Clamp01(volumeScale));
			this.caches[key].lastPlayedTime = Time.realtimeSinceStartup;
		}

		public void Stop()
		{
			this.audioSource.Stop();
		}

		public float GetLength(string key)
		{
			return !this.caches.ContainsKey(key) || this.caches[key].audioClip == null
				? 0f
				: this.caches[key].audioClip.length;
		}

		#endregion
	}
}