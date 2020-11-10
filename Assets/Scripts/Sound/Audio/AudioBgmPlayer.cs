using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

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
			}
			ServiceLocator.Instance.Register<ISoundBgmProvider>(instance);
		}


		#region ISoundBgmProvider

		public bool IsPlaying {
			get;
			private set;
		}

		public bool IsLoop {
			get;
			set;
		}

		public float Volume {
			get;
			set;
		}

		public float Pan {
			get;
			set;
		}

		public float FadeTime {
			get;
			set;
		}

		public float Time {
			get;
			set;
		}

		public void Load(string key, int group = 0)
		{
		}

		public void Load(IEnumerable<string> keys, int group = 0)
		{
		}

		public void Unload(int group = 0)
		{
		}

		public void Play(string key)
		{
		}

		public void Stop()
		{
		}

		public void Pause()
		{
		}

		public float GetLength(string key)
		{
			return 0f;
		}

		#endregion
	}
}