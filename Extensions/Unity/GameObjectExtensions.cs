using UnityEngine;

namespace Unitils
{
	public static class GameObjectExtensions
	{
		public static void SafeActive(this GameObject self, bool isActive)
		{
			if (self != null) self.SetActive(isActive);
		}

		public static void SafeDestory(this GameObject self)
		{
			if (self != null) Object.Destroy(self);
		}

		public static T GetOrAddComponent<T>(this GameObject self) where T : Component
		{
			if (self == null) return null;

			T component = self.GetComponent<T>();
			if (component == null) {
				component = self.AddComponent<T>();
			}
			return component;
		}

		public static void SetTag(this GameObject self, string tagName, bool isChildren)
		{
			self.tag = tagName;
			if (isChildren) {
				foreach (Transform child in self.transform) {
					child.gameObject.SetTag(tagName, isChildren);
				}
			}
		}

		public static void SetLayer(this GameObject self, string layerName, bool isChildren)
		{
			self.SetLayer(LayerMask.NameToLayer(layerName), isChildren);
		}

		public static void SetLayer(this GameObject self, int layer, bool isChildren)
		{
			self.layer = layer;
			if (isChildren) {
				foreach (Transform child in self.transform) {
					child.gameObject.SetLayer(layer, isChildren);
				}
			}
		}
	}
}