using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace Unitils
{
	public static class EditorTools
	{
		public static readonly List<string> ignoreFileExtensions = new List<string> { ".meta", ".DS_Store" };

		public static bool IsIgnoreFile(string filePath) => ignoreFileExtensions.Contains(Path.GetExtension(filePath));

		public static string ToAbsolutePath(string assetPath) => Application.dataPath + assetPath.Replace("Assets", "");
		public static string ToAssetPath(string absolutePath) => absolutePath.Replace(Application.dataPath, "Assets");


		public static string GetSelectionFolder()
		{
			string[] assetGUIDs = Selection.assetGUIDs;
			if (assetGUIDs == null || assetGUIDs.Length <= 0) return "Assets/";

			string assetPath = AssetDatabase.GUIDToAssetPath(assetGUIDs[0]);

			return string.IsNullOrEmpty(Path.GetExtension(assetPath)) ? assetPath : Path.GetDirectoryName(assetPath);
		}

		public static List<string> GetSelectionAssets(Func<string, bool> predicate)
		{
			return GetSelectionAssets(predicate, SearchOption.AllDirectories);
		}

		public static List<string> GetSelectionAssets(Func<string, bool> predicate, SearchOption searchOption)
		{
			List<string> targets = new List<string>();

			string[] assetGUIDs = Selection.assetGUIDs;
			if (assetGUIDs == null || assetGUIDs.Length <= 0) return targets;

			foreach (string assetGuid in assetGUIDs) {
				string assetPath = AssetDatabase.GUIDToAssetPath(assetGuid);
				if (string.IsNullOrEmpty(Path.GetExtension(assetPath))) {
					targets.AddRange(GetAssetFiles(assetPath, searchOption, predicate));
					continue;
				}
				if (predicate != null && !predicate(assetPath)) continue;
				targets.Add(assetPath);
			}
			return targets.Distinct().ToList();
		}

		public static List<UnityEngine.Object> GetSelectionAssets<T>() where T : UnityEngine.Object
		{
			return new List<UnityEngine.Object>(Selection.GetFiltered<T>(SelectionMode.Assets));
		}


		public static void CopyFiles(string source, string destination, bool isOverwrite, SearchOption searchOption)
		{
			string[] files = Directory.GetFiles(source, "*", searchOption);
			string outputFilePath;

			foreach (string sourceFilePath in files) {
				if (IsIgnoreFile(sourceFilePath)) continue;

				outputFilePath = destination + sourceFilePath.Replace(source, "");
				if (!Directory.Exists(outputFilePath)) Directory.CreateDirectory(outputFilePath);

				if (File.Exists(outputFilePath)) {
					if (isOverwrite) {
						File.Delete(outputFilePath);
					}
					else continue;
				}
				File.Copy(sourceFilePath, outputFilePath);
			}
		}

		public static List<string> GetAssetFiles(string assetPath, SearchOption searchOption, Func<string, bool> predicate = null)
		{
			List<string> assetFiles = new List<string>();
			string absolutePath = ToAbsolutePath(assetPath);

			if (!Directory.Exists(absolutePath)) return new List<string>();

			string[] files = Directory.GetFiles(absolutePath, "*", searchOption);
			foreach (string file in files) {
				string assetFile = ToAssetPath(file);
				if (IsIgnoreFile(assetFile)) continue;
				if (predicate != null && !predicate(assetFile)) continue;
				assetFiles.Add(assetFile);
			}
			return assetFiles;
		}
	}
}