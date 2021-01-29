using UnityEngine;

namespace Unitils
{
	public class AudioSourceBgm : MonoBehaviour
	{
		private AudioSource audioSource;
		private float fadeInTime;
		private float fadeOutTime;
		private BGMState state;

		public bool IsRuntimeFadeOut { protected set; get; }


		private void Awake()
		{
			this.audioSource = this.gameObject.GetOrAddComponent<AudioSource>();
			this.audioSource.playOnAwake = false;
		}

		private void Update()
		{
			if (this.state != null) {
				this.state.Update();
			}
		}


		public void Play(AudioClip audioClip)
		{
			if (audioClip == null) return;

			this.audioSource.clip = audioClip;
			this.state = new WaitState(this);
			this.state.Play();
		}

		public void Stop(float fadeTime)
		{
			this.fadeOutTime = fadeTime;
			this.state.Stop();
			this.state = null;
		}

		public void Pause()
		{
			this.state.Pause();
		}

		public void UnPause()
		{
			this.state.Play();
		}


		#region State

		class BGMState
		{
			protected AudioSourceBgm self;

			public BGMState(AudioSourceBgm bgmPlayer)
			{
				this.self = bgmPlayer;
			}

			public virtual void Play() {}
			public virtual void Pause() {}
			public virtual void Stop() {}
			public virtual void Update() {}
		}

		class WaitState : BGMState
		{
			public WaitState(AudioSourceBgm bgmPlayer) : base(bgmPlayer) {}

			public override void Play()
			{
				this.self.IsRuntimeFadeOut = true;
				if (this.self.fadeInTime > 0f) {
					this.self.state = new FadeInState(this.self);
				}
				else {
					this.self.state = new PlayingState(this.self);
				}
			}
		}

		class FadeInState : BGMState
		{
			private float timer;

			public FadeInState(AudioSourceBgm bgmPlayer) : base(bgmPlayer)
			{
				this.self.audioSource.Play();
				this.self.audioSource.volume = 0f;
			}

			public override void Pause()
			{
				this.self.state = new PauseState(this.self, this);
			}

			public override void Stop()
			{
				this.self.state = new FadeOutState(this.self);
			}

			public override void Update()
			{
				this.timer += Time.unscaledDeltaTime;
				this.self.audioSource.volume = this.timer / this.self.fadeInTime;

				if (this.timer >= this.self.fadeInTime) {
					this.self.state = new PlayingState(this.self);
				}
			}
		}

		class PlayingState : BGMState
		{
			public PlayingState(AudioSourceBgm bgmPlayer) : base(bgmPlayer)
			{
				if (!this.self.audioSource.isPlaying) {
					this.self.audioSource.Play();
				}
			}

			public override void Pause()
			{
				this.self.state = new PauseState(this.self, this);
			}

			public override void Stop()
			{
				this.self.state = new FadeOutState(this.self);
			}
		}

		class PauseState : BGMState
		{
			private readonly BGMState beforeState;

			public PauseState(AudioSourceBgm bgmPlayer, BGMState beforeState) : base(bgmPlayer)
			{
				this.beforeState = beforeState;
				this.self.audioSource.Pause();
			}

			public override void Stop()
			{
				this.self.audioSource.Stop();
				this.self.state = new WaitState(this.self);
			}

			public override void Play()
			{
				this.self.state = this.beforeState;
				this.self.audioSource.UnPause();
			}
		}

		class FadeOutState : BGMState
		{
			private float timer;

			public FadeOutState(AudioSourceBgm bgmPlayer) : base(bgmPlayer) {}

			public override void Pause()
			{
				this.self.state = new PauseState(this.self, this);
			}

			public override void Update()
			{
				this.timer += Time.unscaledDeltaTime;
				this.self.audioSource.volume = 1f - (this.timer / this.self.fadeOutTime);

				if (this.timer >= this.self.fadeOutTime) {
					this.self.audioSource.volume = 0f;
					this.self.IsRuntimeFadeOut = false;
					this.self.audioSource.Stop();
					this.self.state = new WaitState(this.self);
				}
			}
		}

		#endregion
	}
}