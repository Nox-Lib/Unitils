using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Unitils
{
	public class AudioBgmPlayer : MonoBehaviour, ISoundBgmProvider
	{
		private static AudioBgmPlayer instance = null;

		public static void Activation()
		{
			if (instance == null) {
				instance = new GameObject("AudioBgmPlayer", typeof(AudioBgmPlayer)).GetComponent<AudioBgmPlayer>();
				SceneManager.MoveGameObjectToScene(instance.gameObject, SceneManager.GetSceneByName(Define.SceneType.Unitils.Name));
				instance.audioSources = new AudioSourceBgm[2];
			}
			ServiceLocator.Instance.Register<ISoundBgmProvider>(instance);
		}


		private AudioSourceBgm[] audioSources;


		private class Cache
		{
			public List<int> groups = new List<int>();
			public AudioClip audioClip;
		}

		private readonly Dictionary<string, Cache> caches = new Dictionary<string, Cache>();


		private float volume = 1f;
		private float pan = 0f;
		private float fadeTime = 0.5f;


		#region ISoundBgmProvider

		public bool IsPlaying => this.audioSources[0] != null && this.audioSources[0].IsPlaying;

		public float Volume {
			get { return this.volume; }
			set {
				this.volume = value;
				if (this.audioSources[0] != null) this.audioSources[0].Volume = this.volume;
				if (this.audioSources[1] != null) this.audioSources[1].Volume = this.volume;
			}
		}

		public float Pan {
			get { return this.pan; }
			set {
				this.pan = value;
				if (this.audioSources[0] != null) this.audioSources[0].Pan = this.pan;
				if (this.audioSources[1] != null) this.audioSources[1].Pan = this.pan;
			}
		}

		public float FadeTime {
			get { return this.fadeTime; }
			set {
				this.fadeTime = value;
				if (this.audioSources[0] != null) this.audioSources[0].FadeTime = this.fadeTime;
				if (this.audioSources[1] != null) this.audioSources[1].FadeTime = this.fadeTime;
			}
		}

		public float Time {
			get {
				return this.audioSources[0] != null ? this.audioSources[0].Time : 0f;
			}
			set {
				if (this.audioSources[0] != null) this.audioSources[0].Time = value;
			}
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
			this.Idle();

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

		public void Play(string key, bool isLoop)
		{
			if (!this.caches.ContainsKey(key) || this.caches[key].audioClip == null) return;

			if (this.audioSources[0] != null && this.audioSources[0].PlayClipName == this.caches[key].audioClip.name) {
				this.audioSources[0].Play(this.caches[key].audioClip, isLoop);
				return;
			}
			if (this.audioSources[1] != null) {
				this.audioSources[1].Stop();
				this.audioSources[1].DestoryGameObject();
			}
			if (this.audioSources[0] != null) {
				this.audioSources[1] = this.audioSources[0];
				this.audioSources[1].gameObject.name = "AudioSourceBgm_FadeOut";
				this.audioSources[1].Stop();
			}
			this.audioSources[0] = Utility.Unity.CreateGameObject<AudioSourceBgm>("AudioSourceBgm", instance.transform);
			this.audioSources[0].Volume = this.Volume;
			this.audioSources[0].Pan = this.Pan;
			this.audioSources[0].FadeTime = this.FadeTime;
			this.audioSources[0].Play(this.caches[key].audioClip, isLoop);
		}

		public void Stop()
		{
			if (this.audioSources[0] != null) this.audioSources[0].Stop();
			if (this.audioSources[1] != null) this.audioSources[1].Stop();
		}

		public void Pause()
		{
			if (this.audioSources[0] != null) this.audioSources[0].Pause();
			if (this.audioSources[1] != null) this.audioSources[1].Pause();
		}

		public void UnPause()
		{
			if (this.audioSources[0] != null) this.audioSources[0].UnPause();
			if (this.audioSources[1] != null) this.audioSources[1].UnPause();
		}

		public float GetLength(string key)
		{
			return !this.caches.ContainsKey(key) || this.caches[key].audioClip == null
				? 0f
				: this.caches[key].audioClip.length;
		}

		#endregion


		private void Idle()
		{
			float _fadeTime = this.fadeTime;
			this.FadeTime = 0f;
			this.Stop();
			if (this.audioSources[0] != null) this.audioSources[0].DestoryGameObject();
			if (this.audioSources[1] != null) this.audioSources[1].DestoryGameObject();
			this.FadeTime = _fadeTime;
		}
	}
}