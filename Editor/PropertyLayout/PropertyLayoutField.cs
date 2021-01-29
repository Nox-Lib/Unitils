using UnityEngine;
using UnityEditor;

namespace Unitils
{
	public class PropertyLayoutField : IPropertyLayout
	{
		public SerializedProperty[] Properties { get; }
		public SerializedProperty Property => this.Properties[0];
		public GUIContent Label { get; }

		public void Draw()
		{
			EditorGUILayout.PropertyField(this.Property, this.Label);
		}

		public void SetValues(params object[] args) {}

		public PropertyLayoutField(string label, SerializedProperty serializedProperty)
		{
			this.Properties = new SerializedProperty[] { serializedProperty };
			this.Label = new GUIContent(label);
		}

		public PropertyLayoutField(string label, SerializedObject serializedObject, string propertyName)
		{
			this.Properties = new SerializedProperty[] { serializedObject.FindProperty(propertyName) };
			this.Label = new GUIContent(label);
		}
	}
}