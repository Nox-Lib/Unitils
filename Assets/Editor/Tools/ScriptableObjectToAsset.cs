using System.IO;
using UnityEngine;
using UnityEditor;

namespace Unitils
{
	public static class ScriptableObjectToAsset
	{
		private static readonly string[] labels = { "Data", "ScriptableObject", string.Empty };

		[MenuItem("Assets/Create ScriptableObject")]
		private static void Create()
		{
			foreach (Object selectedObject in Selection.objects) {
				string path = GetSavePath(selectedObject);

				ScriptableObject obj = ScriptableObject.CreateInstance(selectedObject.name);
				AssetDatabase.CreateAsset(obj, path);
				labels[2] = selectedObject.name;

				ScriptableObject sobj = AssetDatabase.LoadAssetAtPath(path, typeof(ScriptableObject)) as ScriptableObject;
				AssetDatabase.SetLabels(sobj, labels);
				EditorUtility.SetDirty(sobj);
			}
		}

		private static string GetSavePath(Object selectedObject)
		{
			string objectName = selectedObject.name;
			string dirPath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(selectedObject));
			string path = string.Format("{0}/{1}.asset", dirPath, objectName);

			if (File.Exists(path)) {
				for (int i = 1; ; i++) {
					path = string.Format("{0}/{1}({2}).asset", dirPath, objectName, i);
					if (!File.Exists(path)) {
						break;
					}
				}
			}
			return path;
		}

		public static T Create<T>(string outputPath) where T : ScriptableObject
		{
			System.Type type = typeof(T);

			ScriptableObject obj = ScriptableObject.CreateInstance(type);
			AssetDatabase.CreateAsset(obj, outputPath);
			labels[2] = type.Name;

			ScriptableObject sobj = AssetDatabase.LoadAssetAtPath(outputPath, typeof(ScriptableObject)) as ScriptableObject;
			AssetDatabase.SetLabels(sobj, labels);
			EditorUtility.SetDirty(sobj);

			return sobj as T;
		}
	}
}