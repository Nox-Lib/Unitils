using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Unitils
{
	public class SimpleFader : MonoBehaviour, IFadeProvider
	{
		private static SimpleFader instance = null;

		public static void Activation()
		{
			if (instance == null) {
				instance = Instantiate(Resources.Load<GameObject>("Prefabs/SimpleFader")).GetComponent<SimpleFader>();
				instance.name = typeof(SimpleFader).Name;
				DontDestroyOnLoad(instance.gameObject);
			}
			ServiceLocator.Instance.Register<IFadeProvider>(instance);
		}


		public class Configuration
		{
			public float duration;
			public Color color;

			public Configuration()
			{
				this.duration = 0.5f;
				this.color = Color.black;
			}
		}

		[SerializeField] private Image fadeImage;

		private Configuration configuration = new Configuration();
		private Tweener fadeTweener;

		private void Awake()
		{
			this.fadeImage.SafeEnabled(false);
			this.fadeImage.SetColor(this.configuration.color);
			this.fadeImage.SetAlpha(0f);
		}

		#region IFadeProvider

		public bool IsRunning => this.fadeTweener != null;

		public void SetConfiguration(object arg)
		{
			this.configuration = (arg as Configuration) ?? this.configuration;
			this.fadeImage.SetColor(this.configuration.color);
		}

		public void SetFactor(float factor)
		{
			factor = Mathf.Clamp01(factor);
			if (this.fadeTweener != null) this.fadeImage.SetAlpha(factor);
		}

		public void FadeIn()
		{
			if (this.fadeTweener != null) this.fadeTweener.Kill();

			this.fadeTweener = this.fadeImage
				.DOFade(0f, this.configuration.duration)
				.OnComplete(() =>
				{
					this.fadeTweener = null;
					this.fadeImage.SafeEnabled(false);
				});
		}

		public void FadeOut()
		{
			if (this.fadeTweener != null) this.fadeTweener.Kill();

			this.fadeImage.SafeEnabled(true);

			this.fadeTweener = this.fadeImage
				.DOFade(1f, this.configuration.duration)
				.OnComplete(() => this.fadeTweener = null);
		}

		#endregion
	}
}