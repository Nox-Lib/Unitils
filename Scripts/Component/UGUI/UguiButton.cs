using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace Unitils
{
	public class UguiButton : UguiEventDetector, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
	{
		private enum State
		{
			Normal = 0,
			Enter,
			Press,
			Disable
		}

		public enum TriggerDownType
		{
			DownOnce = 0,
			DownLong,
			DownKeep,
			DownKeepAcceleration
		}

		public override bool IsInteractable {
			get { return base.IsInteractable; }
			set {
				base.IsInteractable = value;
				this.SetState(this.isInteractable ? State.Normal : State.Disable);
			}
		}

		#if UNITY_EDITOR
		#pragma warning disable 414
		[SerializeField] private int triggerIndex = -1;
		#pragma warning restore 414
		#endif

		[SerializeField] private TriggerDownType triggerDownType = TriggerDownType.DownOnce;
		public TriggerDownType DownType {
			get { return this.triggerDownType; }
			set { this.triggerDownType = value; }
		}

		[SerializeField] private float accelerationTime = 1.3f;
		[SerializeField] private float acceleratedInterval = 0.1f;
		[SerializeField] private bool isPlaySoundOnEveryInvoke = true;

		[SerializeField] private bool isEnabledAnimation = true;
		[SerializeField] private float duration = 0.2f;
		[SerializeField] private float enteredScale = 1f;
		[SerializeField] private float pressedScale = 1f;
		[SerializeField] private Color normalColor = Color.white;
		[SerializeField] private Color enteredColor = new Color32(255, 255, 255, 255);
		[SerializeField] private Color pressedColor = new Color32(180, 180, 180, 255);
		[SerializeField] private Color disabledColor = new Color32(180, 180, 180, 255);

		private bool isStarted;
		private bool isPressed;

		private bool isInvokedDownLong;
		private float pressedTime;
		private float invokedTime;

		private Vector3 baseScale;
		private Tweener changeScaleTweener;
		private UguiColorGroup colorGroup;


		protected void Awake()
		{
			this.baseScale = this.transform.localScale;
		}

		protected void Start()
		{
			this.isStarted = true;
		}

		private void Update()
		{
			bool isDownProcess = this.isPressed
				&& this.trigger == Define.ButtonTrigger.Down
				&& this.triggerDownType != TriggerDownType.DownOnce;

			if (!isDownProcess) return;

			if (this.triggerDownType == TriggerDownType.DownLong) {
				bool isInvoke = (Time.realtimeSinceStartup - this.pressedTime) > this.Interval;
				if (!isInvoke || this.isInvokedDownLong) return;
				this.onEvent.Invoke();
				this.PlaySound();
				this.isInvokedDownLong = true;
				return;
			}

			if (this.triggerDownType == TriggerDownType.DownKeep) {
				float elapsed = Time.realtimeSinceStartup - this.invokedTime;
				if (elapsed < this.Interval) return;
				this.invokedTime = Time.realtimeSinceStartup;
				this.onEvent.Invoke();
				if (this.isPlaySoundOnEveryInvoke) this.PlaySound();
				return;
			}

			if (this.triggerDownType == TriggerDownType.DownKeepAcceleration) {
				float elapsed = Time.realtimeSinceStartup - this.pressedTime;
				float useInterval = elapsed > this.accelerationTime ? this.acceleratedInterval : this.Interval;
				elapsed = Time.realtimeSinceStartup - this.invokedTime;
				if (elapsed < useInterval) return;
				this.invokedTime = Time.realtimeSinceStartup;
				this.onEvent.Invoke();
				if (this.isPlaySoundOnEveryInvoke) this.PlaySound();
				return;
			}
		}

		private void OnDestroy()
		{
			if (this.changeScaleTweener != null) {
				this.changeScaleTweener.Kill();
				this.changeScaleTweener = null;
			}
		}


		#region Animation

		private void SetState(State state)
		{
			if (!this.isStarted || !this.isEnabledAnimation) {
				return;
			}

			switch (state) {
				case State.Normal:
					this.SetScale(1f);
					this.SetColor(this.normalColor);
					break;
				case State.Enter:
					this.SetScale(this.enteredScale);
					this.SetColor(this.enteredColor);
					break;
				case State.Press:
					this.SetScale(this.pressedScale);
					this.SetColor(this.pressedColor);
					break;
				case State.Disable:
					this.SetScale(1f);
					this.SetColor(this.disabledColor);
					break;
			}
		}

		private void SetScale(float scale)
		{
			Vector3 from = this.transform.localScale;
			Vector3 to = this.baseScale * scale;

			if (this.changeScaleTweener != null) {
				this.changeScaleTweener.Kill();
			}

			this.changeScaleTweener = this.transform
				.DOScale(scale, this.duration)
				.SetEase(Ease.Linear)
				.OnComplete(() => this.changeScaleTweener = null);
		}

		private void SetColor(Color color)
		{
			if (this.colorGroup == null) {
				this.colorGroup = this.gameObject.GetOrAddComponent<UguiColorGroup>();
				this.colorGroup.Add(this.gameObject);
			}
			this.colorGroup.SetColor(color, this.duration);
		}
		#endregion


		#region Event

		public void OnPointerClick(PointerEventData eventData)
		{
			if (this.trigger == Define.ButtonTrigger.Click) {
				this.onEvent.Invoke();
				this.PlaySound();
			}
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			this.isPressed = true;
			this.SetState(State.Press);
			if (this.trigger == Define.ButtonTrigger.Down) {
				if (this.triggerDownType == TriggerDownType.DownLong) {
					this.isInvokedDownLong = false;
				}
				else {
					this.onEvent.Invoke();
					this.PlaySound();
				}
				this.pressedTime = Time.realtimeSinceStartup;
				this.invokedTime = this.pressedTime;
			}
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			this.isPressed = this.isInvokedDownLong = false;
			this.SetState(State.Normal);
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			if (this.isPressed) {
				this.SetState(State.Press);
			}
			else {
				this.SetState(State.Enter);
			}
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			if (this.isPressed) return;
			this.SetState(State.Normal);
		}
		#endregion
	}
}