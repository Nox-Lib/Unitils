using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace Unitils
{
	public class MultitapControlGraphicRaycaster : GraphicRaycaster
	{
		public bool isSingleTapMode = true;

		public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
		{
			if (this.isSingleTapMode && eventData.pointerId >= 1) {
				return;
			}
			base.Raycast(eventData, resultAppendList);
		}

		public void SetLayerMask(params string[] layerNames)
		{
			int layerMask = 0;
			for (int i = 0; i < layerNames.Length; i++) {
				layerMask |= 1 << LayerMask.NameToLayer(layerNames[i]);
			}
			this.m_BlockingMask = layerMask;
		}
	}
}