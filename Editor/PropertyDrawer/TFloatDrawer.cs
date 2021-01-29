using UnityEngine;
using UnityEditor;

namespace Unitils
{
	[CustomPropertyDrawer(typeof(TFloat))]
	public class TFloatDrawer : PropertyDrawer
	{
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return EditorGUI.GetPropertyHeight(property, label, true);
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			SerializedProperty valueProperty = property.FindPropertyRelative("value");
			EditorGUI.PropertyField(position, valueProperty, label, true);
		}
	}
}