using UnityEngine;
using DG.Tweening;

namespace Unitils
{
	public abstract class ColorGroupComponentBase : MonoBehaviour
	{
		[SerializeField] protected Color color = Color.white;
		public Color Current { get { return this.color; } }

		protected struct ColorData<T>
		{
			public T Target { get; }
			public Color Source { get; }

			public ColorData(T target, Color source)
			{
				this.Target = target;
				this.Source = source;
			}
		}

		private Tweener changeColorTweener;


		protected abstract void Prepare();
		protected abstract void SetColor();
		public abstract void Clear();


		private void Awake()
		{
			this.Prepare();
		}

		private void OnDestroy()
		{
			this.changeColorTweener?.Kill();
			this.changeColorTweener = null;
		}


		public void SetAlpha(float alpha)
		{
			Color color = this.color;
			color.a = Mathf.Clamp01(alpha);
			this.SetColor(color);
		}

		public void SetColor(Color color)
		{
			this.changeColorTweener?.Kill();
			this.changeColorTweener = null;
			this.color = color;
			this.SetColor();
		}

		public void SetColor(Color newColor, float duration)
		{
			Color oldColor = this.color;
			this.changeColorTweener?.Kill();

			this.changeColorTweener = DOTween.To(
				() => oldColor,
				setter => {
					this.color = setter;
					this.SetColor();
				},
				newColor,
				duration
			);
			this.changeColorTweener.SetAutoKill().OnComplete(() => this.changeColorTweener = null);
		}

		public void SetAlpha(float alpha, float duration)
		{
			Color color = this.color;
			color.a = Mathf.Clamp01(alpha);
			this.SetColor(color, duration);
		}
	}
}