using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace Unitils
{
	public class MultitapControlPhysicsRaycaster : PhysicsRaycaster
	{
		public bool isSingleTapMode = true;

		public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
		{
			if (this.isSingleTapMode && eventData.pointerId >= 1) {
				return;
			}
			base.Raycast(eventData, resultAppendList);
		}
	}
}