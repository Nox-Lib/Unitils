using UnityEngine;

namespace Unitils
{
	public class SystemData : ScriptableObject, IScreenData
	{
		private static SystemData instance = null;
		public static SystemData Instance {
			get {
				return instance = instance ??= Resources.Load<SystemData>(DefineData.SYSTEM_DATA);
			}
		}

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

		[SerializeField] private Define.ScreenMode screenMode = Define.ScreenMode.Expand;
		[SerializeField] private Vector2 baseScreenSize = new Vector2(1334f, 750f);
		[SerializeField] private Vector2 minScreenSize = new Vector2(1334f, 750f);
		[SerializeField] private Vector2 maxScreenSize = new Vector2(1624f, 1000f);
		[SerializeField] private bool isScreenEdgeMask = true;
		[SerializeField] private Color screenEdgeMaskColor = Color.black;

		public Define.ScreenMode ScreenMode => this.screenMode;
		public Vector2 BaseScreenSize => this.baseScreenSize;
		public Vector2 MinScreenSize => this.minScreenSize;
		public Vector2 MaxScreenSize => this.maxScreenSize;
		public bool IsScreenEdgeMask => this.isScreenEdgeMask;
		public Color ScreenEdgeMaskColor => this.screenEdgeMaskColor;
		#endregion
	}
}