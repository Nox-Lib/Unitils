using UnityEngine;
using UnityEngine.UI;

namespace Unitils
{
	[ExecuteAlways, RequireComponent(typeof(CanvasScaler))]
	public class UguiCanvasScaleAdjuster : MonoBehaviour
	{
		private void Start()
		{
			SystemData systemData = SystemData.Instance;
			CanvasScaler canvasScaler = this.GetComponent<CanvasScaler>();
			canvasScaler.referenceResolution = systemData.BaseScreenSize;
			canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
			canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
		}
	}
}