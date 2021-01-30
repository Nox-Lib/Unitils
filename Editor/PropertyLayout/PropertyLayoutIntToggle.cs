using UnityEngine;
using UnityEditor;

namespace Unitils
{
	public class PropertyLayoutIntToggle : IPropertyLayout
	{
		public SerializedProperty[] Properties { get; }
		public SerializedProperty Property => this.Properties[0];
		public GUIContent Label { get; }

		public void Draw()
		{
			this.Property.intValue = EditorGUILayout.Toggle(this.Label, this.Property.intValue > 0) ? 1 : 0;
		}

		public void SetValues(params object[] args) {}

		public PropertyLayoutIntToggle(string label, SerializedProperty serializedProperty)
		{
			this.Properties = new SerializedProperty[] { serializedProperty };
			this.Label = new GUIContent(label);
		}

		public PropertyLayoutIntToggle(string label, SerializedObject serializedObject, string propertyName)
		{
			this.Properties = new SerializedProperty[] { serializedObject.FindProperty(propertyName) };
			this.Label = new GUIContent(label);
		}
	}
}
