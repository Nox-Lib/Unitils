﻿using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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
	}
}