using UnityEngine;
using UnityEditor;
using System;
using System.Linq;

namespace Unitils
{
	[CustomPropertyDrawer(typeof(PullDownAttribute))]
	public class PullDownDrawer : PropertyDrawer
	{
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return EditorGUI.GetPropertyHeight(property, label, true);
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			PullDownAttribute pullDownAttribute = this.attribute as PullDownAttribute;
			int index = 0;

			if (pullDownAttribute.isInt) {
				index = Mathf.Max(0, Array.IndexOf(pullDownAttribute.intArray, property.intValue));
				index = EditorGUI.Popup(position, label.text, index, pullDownAttribute.intArray.Select(x => x.ToString()).ToArray());
				property.intValue = pullDownAttribute.intArray[index];
				return;
			}

			if (pullDownAttribute.isFloat) {
				index = Mathf.Max(0, Array.IndexOf(pullDownAttribute.floatArray, property.floatValue));
				index = EditorGUI.Popup(position, label.text, index, pullDownAttribute.floatArray.Select(x => x.ToString()).ToArray());
				property.floatValue = pullDownAttribute.floatArray[index];
				return;
			}

			if (pullDownAttribute.isString) {
				index = Mathf.Max(0, Array.IndexOf(pullDownAttribute.stringArray, property.stringValue));
				index = EditorGUI.Popup(position, label.text, index, pullDownAttribute.stringArray);
				property.stringValue = pullDownAttribute.stringArray[index];
				return;
			}
		}
	}
}