using System;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D;

namespace Unitils
{
	public class AtlasImageSelectionWindow : SelectionWindowBase
	{
		private string[] iconAssetPaths;
		protected override string[] IconAssetPaths => this.iconAssetPaths;

		private string[] iconLabels;
		protected override string[] IconLabels => this.iconLabels;

		protected override Texture2D LoadIconTexture(string assetPath)
		{
			Sprite sprite = this.spriteAtlas.GetSprite(assetPath);
			return sprite != null ? sprite.texture : null;
		}

		private SpriteAtlas spriteAtlas;

		public static AtlasImageSelectionWindow Open(SpriteAtlas spriteAtlas, string defaultValue = null, Action<string> onSelect = null)
		{
			AtlasImageSelectionWindow window = GetWindowWithRect<AtlasImageSelectionWindow>(new Rect(0, 0, 660, 600), true);

			window.spriteAtlas = spriteAtlas;

			Sprite[] sprites = new Sprite[spriteAtlas.spriteCount];
			spriteAtlas.GetSprites(sprites);

			string[] spriteNames = sprites.Select(_ => _.name.Replace("(Clone)", "")).ToArray();
			window.iconAssetPaths = spriteNames;
			window.iconLabels = spriteNames;

			window.maxSize = new Vector2(1600, 1200);
			window.minSize = new Vector2(400, 300);
			window.itemSize = new Vector2(100f, 100f);
			window.spacing = new Vector2(5f, 5f);

			window.titleContent = new GUIContent("Sprite Selection");
			window.onSelect = onSelect;
			window.SetSearchValue(defaultValue);

			window.ShowAuxWindow();

			window.isInitialized = true;

			return window;
		}
	}
}