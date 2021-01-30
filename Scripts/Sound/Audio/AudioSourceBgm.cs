using UnityEngine;

namespace Unitils
{
	public class AudioSourceBgm : MonoBehaviour
	{
		private AudioSource audioSource;
		private BGMState state;
		private float volume = 1f;
		private float volumeFadeScale = 1f;

		public bool IsRuntimeFade => this.state is FadeInState || this.state is FadeOutState;
		public bool IsPlaying => this.state is FadeInState || this.state is PlayingState;
		public string PlayClipName => this.audioSource.clip != null ? this.audioSource.clip.name : "";

		public float Volume {
			get { return this.volume; }
			set {
				this.volume = Mathf.Clamp01(value);
				this.audioSource.volume = Mathf.Clamp01(this.volume * this.volumeFadeScale);
			}
		}

		public float Pan {
			get { return this.audioSource.panStereo; }
			set { this.audioSource.panStereo = Mathf.Clamp(value, -1f, 1f); }
		}

		public float FadeTime { get; set; } = 0.5f;

		public float Time {
			get { return this.audioSource.time; }
			set { this.audioSource.time = value; }
		}


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


		public void Play(AudioClip audioClip, bool isLoop)
		{
			if (audioClip == null) return;

			if (this.PlayClipName == audioClip.name) {
				if (this.IsPlaying) return;
			}
			else {
				this.audioSource.clip = audioClip;
			}

			if (!(this.state is FadeOutState)) this.audioSource.volume = 0f;

			this.audioSource.loop = isLoop;
			this.state = new WaitState(this);
			this.state.Play();
		}

		public void Stop()
		{
			if (this.state == null) return;
			this.state.Stop();
		}

		public void Pause()
		{
			if (this.state == null) return;
			this.state.Pause();
		}

		public void UnPause()
		{
			if (this.state == null) return;
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
				if (this.self.FadeTime > 0f) {
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
			private float startVolume;

			public FadeInState(AudioSourceBgm bgmPlayer) : base(bgmPlayer)
			{
				this.self.audioSource.Play();
				this.startVolume = this.self.audioSource.volume;
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
				this.timer += UnityEngine.Time.unscaledDeltaTime;
				this.self.volumeFadeScale = Mathf.Max(this.startVolume, this.timer / this.self.FadeTime);
				this.self.audioSource.volume = Mathf.Clamp01(this.self.volume * this.self.volumeFadeScale);

				if (this.timer >= this.self.FadeTime) {
					this.self.volumeFadeScale = 1f;
					this.self.audioSource.volume = 1f;
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
			private float startVolume;

			public FadeOutState(AudioSourceBgm bgmPlayer) : base(bgmPlayer)
			{
				this.startVolume = this.self.audioSource.volume;
			}

			public override void Pause()
			{
				this.self.state = new PauseState(this.self, this);
			}

			public override void Update()
			{
				this.timer += UnityEngine.Time.unscaledDeltaTime;
				this.self.volumeFadeScale = Mathf.Min(this.startVolume, 1f - (this.timer / this.self.FadeTime));
				this.self.audioSource.volume = Mathf.Clamp01(this.self.volume * this.self.volumeFadeScale);

				if (this.timer >= this.self.FadeTime) {
					this.self.volumeFadeScale = 0f;
					this.self.audioSource.volume = 0f;
					this.self.audioSource.Stop();
					this.self.state = new WaitState(this.self);
				}
			}
		}

		#endregion
	}
}