using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Unitils
{
	public class UguiColorGroup : ColorGroupComponentBase
	{
		[SerializeField] private List<Graphic> graphics = new List<Graphic>();
		private List<ColorData<Graphic>> graphicColors = new List<ColorData<Graphic>>();
		private bool isPrepared = false;


		protected override void Prepare()
		{
			this.isPrepared = true;
			if (this.graphics == null) return;
			this.graphicColors = new List<ColorData<Graphic>>();
			this.graphics.ForEach(x => this.graphicColors.Add(new ColorData<Graphic>(x, x.color)));
		}

		protected override void SetColor()
		{
			this.graphicColors.ForEach(x => x.Target.SetColor(this.color));
		}

		public override void Clear()
		{
			this.graphics.Clear();
			this.graphicColors.Clear();
		}


		public void Add(Graphic graphic)
		{
			if (!this.isPrepared) this.Prepare();
			if (graphic == null) return;
			this.graphics.Add(graphic);
			this.graphicColors.Add(new ColorData<Graphic>(graphic, graphic.color));
		}

		public void Add(IEnumerable<Graphic> graphics)
		{
			if (graphics == null) return;
			foreach (Graphic graphic in graphics) {
				this.Add(graphic);
			}
		}

		public void Add(GameObject target)
		{
			if (target != null) this.Add(target.GetComponentsInChildren<Graphic>(true));
		}



		#if UNITY_EDITOR

		[CustomEditor(typeof(UguiColorGroup))]
		private class InspectorEditor : Editor
		{
			public override void OnInspectorGUI()
			{
				base.OnInspectorGUI();

				EditorGUILayout.Space();

				if (GUILayout.Button("Apply")) {
					UguiColorGroup colorGroup = this.target as UguiColorGroup;
					if (colorGroup != null) {
						colorGroup.Prepare();
						colorGroup.SetColor();
					}
				}
				if (GUILayout.Button("子オブジェクトからGraphicを収集")) {
					UguiColorGroup colorGroup = this.target as UguiColorGroup;
					if (colorGroup != null) {
						colorGroup.Prepare();
						colorGroup.Clear();
						colorGroup.Add(colorGroup.gameObject);
					}
				}
			}
		}
		#endif
	}
}