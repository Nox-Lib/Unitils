using System.IO;
using UnityEngine;
using UnityEditor;

namespace Unitils
{
	public static class OpenProjectFolder
	{
		[MenuItem("Unitils/Open Project Folder")]
		private static void Run()
		{
			EditorUtility.RevealInFinder(Path.Combine(Application.dataPath, "../"));
		}
	}
}