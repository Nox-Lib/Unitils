using UnityEngine;
using UnityEditor;

namespace Unitils
{
	public class PropertyLayoutToggleLeft : IPropertyLayout
	{
		public SerializedProperty[] Properties { get; private set; }
		public SerializedProperty Property => this.Properties[0];
		public GUIContent Label { get; private set; }

		public void Draw()
		{
			this.Property.boolValue = EditorGUILayout.ToggleLeft(this.Label, this.Property.boolValue);
		}

		public void SetValues(params object[] args) { }

		public PropertyLayoutToggleLeft(string label, SerializedProperty serializedProperty)
		{
			this.Properties = new SerializedProperty[] { serializedProperty };
			this.Label = new GUIContent(label);
		}

		public PropertyLayoutToggleLeft(string label, SerializedObject serializedObject, string propertyName)
		{
			this.Properties = new SerializedProperty[] { serializedObject.FindProperty(propertyName) };
			this.Label = new GUIContent(label);
		}
	}
}