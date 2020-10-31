using UnityEngine;

namespace Unitils
{
	public class SystemData : ScriptableObject
	{
		#region System

		[SerializeField] private int frameRate = 60;

		public int FrameRate => this.frameRate;

		public void SetFrameRate(int frameRate)
		{
			this.frameRate = frameRate;
			Application.targetFrameRate = this.FrameRate;
		}
		#endregion


		#region Screen

		[SerializeField] private Define.System.ScreenMode screenMode = Define.System.ScreenMode.Expand;
		[SerializeField] private Vector2 baseScreenSize = new Vector2(1334f, 750f);
		[SerializeField] private Vector2 minScreenSize = new Vector2(1334f, 750f);
		[SerializeField] private Vector2 maxScreenSize = new Vector2(1624f, 1000f);
		[SerializeField] private bool isOutsideScreenMask = true;
		[SerializeField] private Color screenMaskColor = Color.black;

		public Define.System.ScreenMode ScreenMode => this.screenMode;
		public Vector2 BaseScreenSize => this.baseScreenSize;
		public Vector2 MinScreenSize => this.minScreenSize;
		public Vector2 MaxScreenSize => this.maxScreenSize;
		public bool IsOutsideScreenMask => this.isOutsideScreenMask;
		public Color ScreenMaskColor => this.screenMaskColor;
		#endregion
	}
}