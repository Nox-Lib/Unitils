using UnityEngine.EventSystems;

namespace Unitils
{
	public class UguiClickDetector : UguiEventDetector, IPointerClickHandler
	{
		public void OnPointerClick(PointerEventData eventData)
		{
			if (this.trigger == Define.ButtonTrigger.Click) {
				this.onEvent.Invoke();
				this.PlaySound();
			}
		}
	}
}