using UnityEngine;
using UnityEngine.Events;

namespace Unitils
{
	public abstract class UguiEventDetector : ButtonComponentBase
	{
		[SerializeField] protected bool isInteractable = true;
		public virtual bool IsInteractable {
			get { return this.isInteractable; }
			set {
				this.enabled = value;
				this.isInteractable = value;
			}
		}

		[SerializeField] protected UnityEvent onEvent = new UnityEvent();
		public UnityEvent OnEvent { get { return this.onEvent; } }
	}
}