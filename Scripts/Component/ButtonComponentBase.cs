using UnityEngine;

namespace Unitils
{
	public abstract class ButtonComponentBase : MonoBehaviour
	{
		[SerializeField] protected Define.ButtonTrigger trigger;
		[SerializeField] protected Define.ButtonSoundType soundType;

		[SerializeField] [Range(0f, 1f)] private float volume = 1f;
		[SerializeField] [Range(-1f, 1f)] private float pan = 0f;

		[SerializeField] private float interval = 0.6f;

		private ISoundSeProvider sound;
		private ButtonSoundData soundData;


		public Define.ButtonTrigger Trigger {
			get { return this.trigger; }
			set { this.trigger = value; }
		}

		public Define.ButtonSoundType SoundType {
			get { return this.soundType; }
			set { this.soundType = value; }
		}

		public float Volume {
			get { return this.volume; }
			set { this.volume = value; }
		}

		public float Pan {
			get { return this.pan; }
			set { this.pan = value; }
		}

		public float Interval {
			get { return this.interval; }
			set { this.interval = value; }
		}


		protected virtual void PlaySound()
		{
			if (!this.enabled) return;

			this.LoadSound();
			if (this.sound == null || this.soundData == null) return;

			string soundName = this.soundData.Get(this.soundType);
			if (string.IsNullOrEmpty(soundName)) return;

			this.sound.Volume = this.volume;
			this.sound.Pan = this.pan;
			this.sound.Play(soundName);
		}


		private void LoadSound()
		{
			if (this.sound == null) {
				this.sound = ServiceLocator.Instance.GetService<ISoundSeProvider>();
			}
			if (this.soundData == null) {
				this.soundData = ButtonSoundData.Instance;
			}
		}
	}
}