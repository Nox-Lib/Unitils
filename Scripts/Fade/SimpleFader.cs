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
				instance = Utils.Unity.LoadPrefab<SimpleFader>("Prefabs/SimpleFader");
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


		private void Awake()
		{
			this.fadeImage.SafeEnabled(false);
			this.fadeImage.SetColor(this.configuration.color);
			this.fadeImage.SetAlpha(0f);
		}


		#region IFadeProvider

		public bool IsRunning { get; private set; }

		public void SetConfiguration(object arg)
		{
			this.configuration = (arg as Configuration) ?? new Configuration();
			this.fadeImage.SetColor(this.configuration.color);
		}

		public void SetFactor(float factor)
		{
			this.fadeImage.SetAlpha(Mathf.Clamp01(factor));
		}

		public void FadeIn()
		{
			this.IsRunning = true;

			this.fadeImage.DOKill();

			this.fadeImage.DOFade(0f, this.configuration.duration)
				.OnComplete(() =>
				{
					this.IsRunning = false;
					this.fadeImage.SafeEnabled(false);
				});
		}

		public void FadeOut()
		{
			this.IsRunning = true;

			this.fadeImage.SafeEnabled(true);
			this.fadeImage.DOKill();

			this.fadeImage.DOFade(1f, this.configuration.duration).OnComplete(() => this.IsRunning = false);
		}

		#endregion
	}
}