using UnityEngine;
using UnityEditor;

namespace Unitils
{
	public class PropertyLayoutPopup : IPropertyLayout
	{
		private string[] options;

		public SerializedProperty[] Properties { get; private set; }
		public SerializedProperty Property => this.Properties[0];
		public GUIContent Label { get; private set; }

		public void Draw()
		{
			this.Property.intValue = EditorGUILayout.Popup(this.Label.text, this.Property.intValue, this.options);
		}

		public void SetValues(params object[] args)
		{
			this.options = args[0] as string[];
		}

		public PropertyLayoutPopup(string label, SerializedProperty serializedProperty, string[] options)
		{
			this.Properties = new SerializedProperty[] { serializedProperty };
			this.Label = new GUIContent(label);
			this.options = options;
		}

		public PropertyLayoutPopup(string label, SerializedObject serializedObject, string propertyName, string[] options)
		{
			this.Properties = new SerializedProperty[] { serializedObject.FindProperty(propertyName) };
			this.Label = new GUIContent(label);
			this.options = options;
		}
	}
}