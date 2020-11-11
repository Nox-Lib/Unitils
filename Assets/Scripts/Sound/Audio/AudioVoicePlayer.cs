using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Unitils
{
	public class AudioVoicePlayer : MonoBehaviour, ISoundVoiceProvider
	{
		private static AudioVoicePlayer instance = null;

		public static void Activation()
		{
			if (instance == null) {
				instance = new GameObject("AudioVoicePlayer", typeof(AudioVoicePlayer)).GetComponent<AudioVoicePlayer>();
				SceneManager.MoveGameObjectToScene(instance.gameObject, SceneManager.GetSceneByName(Define.SceneType.Unitils.Name));
				instance.audioSource = instance.gameObject.AddComponent<AudioSource>();
				instance.audioSource.playOnAwake = false;
			}
			ServiceLocator.Instance.Register<ISoundVoiceProvider>(instance);
		}


		private class Cache
		{
			public List<int> groups = new List<int>();
			public AudioClip audioClip;
		}

		private readonly Dictionary<string, Cache> caches = new Dictionary<string, Cache>();

		private AudioSource audioSource;


		#region ISoundVoiceProvider

		public bool IsPlaying => this.audioSource.isPlaying;

		public float Volume {
			get { return this.audioSource.volume; }
			set { this.audioSource.volume = Mathf.Clamp01(value); }
		}

		public float Pan {
			get { return this.audioSource.panStereo; }
			set { this.audioSource.panStereo = Mathf.Clamp(value, -1f, 1f); }
		}

		public float Time {
			get { return this.audioSource.time; }
			set { this.audioSource.time = value; }
		}

		public void Load(string key, int group = 0)
		{
			Cache cache = null;
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

		public void Play(string key)
		{
			if (!this.caches.ContainsKey(key) || this.caches[key].audioClip == null) return;
			this.audioSource.clip = this.caches[key].audioClip;
			this.audioSource.Play();
		}

		public void Stop()
		{
			this.audioSource.Stop();
		}

		public void Pause()
		{
			this.audioSource.Pause();
		}

		public void UnPause()
		{
			this.audioSource.UnPause();
		}

		public float GetLength(string key)
		{
			if (!this.caches.ContainsKey(key) || this.caches[key].audioClip == null) {
				return 0f;
			}
			return this.caches[key].audioClip.length;
		}

		#endregion
	}
}