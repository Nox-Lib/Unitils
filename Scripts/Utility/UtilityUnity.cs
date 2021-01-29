using System;
using UnityEngine;

namespace Unitils
{
	public static partial class Utility
	{
		public static class Unity
		{
			public static T CreateGameObject<T>(string name, Transform parent, params Type[] components) where T : Component
			{
				GameObject newObject;

				if (components == null) {
					newObject = new GameObject(name);
				}
				else {
					newObject = new GameObject(name, components);
				}
				newObject.transform.SetParent(parent, false);

				return newObject.GetOrAddComponent<T>();
			}
		}
	}
}