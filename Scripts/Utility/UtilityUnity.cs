using System;
using UnityEngine;

namespace Unitils
{
	public static partial class Utils
	{
		public static class Unity
		{
			public static T CreateGameObject<T>(string name, Transform parent = null, params Type[] components) where T : Component
			{
				GameObject newObject;

				if (components == null) {
					newObject = new GameObject(name);
				}
				else {
					newObject = new GameObject(name, components);
				}
				if (parent != null) {
					newObject.transform.SetParent(parent, false);
				}

				return newObject.GetOrAddComponent<T>();
			}


			public static GameObject LoadPrefab(string resourcePath, Transform parent = null)
			{
				GameObject prefab = Resources.Load<GameObject>(resourcePath);
				GameObject result = UnityEngine.Object.Instantiate(prefab);
				if (parent != null) {
					result.transform.SetParent(parent, false);
				}
				return result;
			}

			public static T LoadPrefab<T>(string resourcePath, Transform parent = null) where T : Component
			{
				GameObject prefab = Resources.Load<GameObject>(resourcePath);
				T result = UnityEngine.Object.Instantiate(prefab).GetComponent<T>();
				if (parent != null) {
					result.transform.SetParent(parent, false);
				}
				return result;
			}
		}
	}
}