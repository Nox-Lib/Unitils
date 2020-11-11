using System;
using UnityEngine;
using UnityEditor;

namespace Unitils
{
	public class PropertyLayoutEnumPopup : IPropertyLayout
	{
		private string[] options;
		private int[] values;

		public SerializedProperty[] Properties { get; private set; }
		public SerializedProperty Property => this.Properties[0];
		public GUIContent Label { get; private set; }

		public void Draw()
		{
			this.Property.intValue = EditorGUILayout.IntPopup(this.Label.text, this.Property.intValue, this.options, this.values);
		}

		public void SetValues(params object[] args)
		{
			this.options = Enum.GetNames(args[0] as Type);
			this.values = Enum.GetValues(args[0] as Type) as int[];
		}

		public PropertyLayoutEnumPopup(string label, SerializedProperty serializedProperty, Type enumType)
		{
			this.Properties = new SerializedProperty[] { serializedProperty };
			this.Label = new GUIContent(label);
			this.options = Enum.GetNames(enumType);
			this.values = Enum.GetValues(enumType) as int[];
		}

		public PropertyLayoutEnumPopup(string label, SerializedObject serializedObject, string propertyName, Type enumType)
		{
			this.Properties = new SerializedProperty[] { serializedObject.FindProperty(propertyName) };
			this.Label = new GUIContent(label);
			this.options = Enum.GetNames(enumType);
			this.values = Enum.GetValues(enumType) as int[];
		}
	}
}