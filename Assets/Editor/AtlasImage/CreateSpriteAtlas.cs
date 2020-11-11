using System.IO;
using UnityEngine.U2D;
using UnityEditor;

namespace Unitils
{
	public static class CreateSpriteAtlas
	{
		[MenuItem("Unitils/Sprite Atlas/Create")]
		[MenuItem("Assets/Unitils/Sprite Atlas/Create")]
		private static void Run()
		{
			string assetPath = Path.Combine(EditorTools.GetSelectionFolder(), "NewSpriteAtlas.spriteatlas");

			if (File.Exists(EditorTools.ToAbsolutePath(assetPath))) return;

			SpriteAtlas spriteAtlas = new SpriteAtlas();

			AssetDatabase.CreateAsset(spriteAtlas, assetPath);

			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}
	}
}