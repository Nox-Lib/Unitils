using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Unitils
{
	public abstract class SelectionWindowBase : EditorWindow
	{
		protected virtual string[] IconAssetPaths => null;
		protected virtual string[] IconLabels => null;

		protected virtual Texture2D LoadIconTexture(string assetPath)
		{
			return AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
		}


		protected Vector2 spacing = new Vector2(0f, 0f);
		protected Vector2 itemSize = new Vector2(64f, 64f);
		protected float labelHeight = 16;

		protected Action<string> onSelect;
		protected bool isInitialized;

		private readonly Dictionary<string, Texture2D> textureMap = new Dictionary<string, Texture2D>();
		private string searchValue;
		private GUIStyle labelStyle;
		private Vector2 scrollPosition = new Vector2(0, 0);


		protected void SetSearchValue(string value)
		{
			this.searchValue = value;
		}

		private void OnGUI()
		{
			if (Event.current.keyCode == KeyCode.Escape) {
				this.Close();
				Event.current.Use();
			}

			if (!this.isInitialized) return;

			GUILayout.BeginHorizontal();

			GUI.SetNextControlName("FilterField");
			this.searchValue = GUILayout.TextField(this.searchValue, new GUIStyle("SearchTextField"));
			GUI.FocusControl("FilterField");

			GUI.enabled = !string.IsNullOrEmpty(this.searchValue);
			if (GUILayout.Button("", new GUIStyle("SearchCancelButton"))) {
				this.searchValue = string.Empty;
			}
			GUI.enabled = true;

			GUILayout.EndHorizontal();

			EditorGUILayout.Space();

			if (this.labelStyle == null) {
				this.labelStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, stretchWidth = true };
			}

			this.scrollPosition = EditorGUILayout.BeginScrollView(this.scrollPosition);

			float marginX = 2f;
			float itemPositionX = marginX;
			float itemPositionY = 0f;

			float viewportStart = this.scrollPosition.y;
			float viewportEnd = this.scrollPosition.y + this.position.height;

			for (int i = 0; i < this.IconLabels.Length; i++) {
				string value = this.IconLabels[i];

				if (!string.IsNullOrEmpty(this.searchValue) && !value.Contains(this.searchValue)) continue;

				if (itemPositionX + this.itemSize.x >= this.position.width - marginX) {
					itemPositionX = 0;
					itemPositionY += this.itemSize.y + this.labelHeight + this.spacing.y;
					GUILayout.Space(this.itemSize.y + this.labelHeight + this.spacing.y);
				}

				if (itemPositionY + this.itemSize.y + this.itemSize.y >= viewportStart && itemPositionY <= viewportEnd) {
					if (!this.textureMap.ContainsKey(this.IconAssetPaths[i])) {
						this.textureMap[this.IconAssetPaths[i]] = this.LoadIconTexture(this.IconAssetPaths[i]);
					}

					Texture2D texture = this.textureMap[this.IconAssetPaths[i]];
					if (GUI.Button(new Rect(itemPositionX, itemPositionY, this.itemSize.x, this.itemSize.y), texture)) {
						this.CloseWithCallback(value);
					}
				}

				Rect rect = new Rect(itemPositionX, itemPositionY + this.itemSize.y, this.itemSize.x, this.labelHeight);
				EditorGUI.LabelField(rect, value, this.labelStyle);

				itemPositionX += this.itemSize.x + this.spacing.x;
			}

			GUILayout.Space(this.itemSize.y + this.labelHeight + this.spacing.y);

			EditorGUILayout.EndScrollView();
		}

		private void CloseWithCallback(string value)
		{
			this.onSelect?.Invoke(value);
			this.Close();
		}
	}
}