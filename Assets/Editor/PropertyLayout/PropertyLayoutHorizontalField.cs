using UnityEngine;
using UnityEditor;

namespace Unitils
{
	public class PropertyLayoutHorizontalField : IPropertyLayout
	{
		public SerializedProperty[] Properties { get; private set; }
		public SerializedProperty Property => this.Properties[0];
		public GUIContent Label { get; private set; }

		public void Draw()
		{
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PropertyField(this.Property, this.Label);
			EditorGUILayout.EndHorizontal();
		}

		public void SetValues(params object[] args) {}

		public PropertyLayoutHorizontalField(string label, SerializedProperty serializedProperty)
		{
			this.Properties = new SerializedProperty[] { serializedProperty };
			this.Label = new GUIContent(label);
		}

		public PropertyLayoutHorizontalField(string label, SerializedObject serializedObject, string propertyName)
		{
			this.Properties = new SerializedProperty[] { serializedObject.FindProperty(propertyName) };
			this.Label = new GUIContent(label);
		}
	}
}