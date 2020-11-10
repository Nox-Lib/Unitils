using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

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
			}
			ServiceLocator.Instance.Register<ISoundVoiceProvider>(instance);
		}


		#region ISoundVoiceProvider


		public bool IsPlaying {
			get;
			private set;
		}

		public float Volume {
			get;
			set;
		}

		public float Pan {
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