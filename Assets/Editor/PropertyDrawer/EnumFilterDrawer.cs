using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Unitils
{
	[CustomPropertyDrawer(typeof(EnumFilterAttribute))]
	public class EnumFilterDrawer : PropertyDrawer
	{
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return EditorGUI.GetPropertyHeight(property, label, true);
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EnumFilterAttribute enumFilterAttribute = this.attribute as EnumFilterAttribute;
			List<string> enumItems;
			List<string> filterItems = enumFilterAttribute.items.ToList();

			if (enumFilterAttribute.isIgnoreMode) {
				enumItems = property.enumNames.ToList();
				enumItems.RemoveAll(x => filterItems.Contains(x));
			}
			else {
				enumItems = new List<string>();
				filterItems.ForEach(x => {
					if (property.enumNames.Contains(x)) {
						enumItems.Add(x);
					}
				});
			}

			if (enumItems.Count <= 0) {
				enumItems = property.enumNames.ToList();
			}

			int index = Mathf.Max(0, enumItems.IndexOf(property.enumNames[property.enumValueIndex]));
			index = EditorGUI.Popup(position, label.text, index, enumItems.ToArray());
			property.enumValueIndex = Mathf.Max(0, Array.IndexOf(property.enumNames, enumItems[index]));
		}
	}
}