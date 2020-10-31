using UnityEngine;
using UnityEditor;

namespace Unitils
{
	[CustomPropertyDrawer(typeof(LabelAttribute))]
	public class LabelDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			LabelAttribute labelAttribute = this.attribute as LabelAttribute;
			EditorGUI.PropertyField(position, property, new GUIContent(labelAttribute.label), true);
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return EditorGUI.GetPropertyHeight(property, true);
		}
	}
}