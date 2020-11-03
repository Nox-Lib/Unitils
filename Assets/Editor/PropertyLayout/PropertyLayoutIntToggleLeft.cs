using UnityEngine;
using UnityEditor;

namespace Unitils
{
	public class PropertyLayoutIntToggleLeft : IPropertyLayout
	{
		public SerializedProperty[] Properties { get; private set; }
		public SerializedProperty Property => this.Properties[0];
		public GUIContent Label { get; private set; }

		public void Draw()
		{
			this.Property.intValue = EditorGUILayout.ToggleLeft(this.Label, this.Property.intValue > 0) ? 1 : 0;
		}

		public void SetValues(params object[] args) { }

		public PropertyLayoutIntToggleLeft(string label, SerializedProperty serializedProperty)
		{
			this.Properties = new SerializedProperty[] { serializedProperty };
			this.Label = new GUIContent(label);
		}

		public PropertyLayoutIntToggleLeft(string label, SerializedObject serializedObject, string propertyName)
		{
			this.Properties = new SerializedProperty[] { serializedObject.FindProperty(propertyName) };
			this.Label = new GUIContent(label);
		}
	}
}
