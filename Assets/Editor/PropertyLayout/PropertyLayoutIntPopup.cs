using UnityEngine;
using UnityEditor;
using System.Linq;

namespace Unitils
{
	public class PropertyLayoutIntPopup : IPropertyLayout
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
			this.options = args[0] as string[];
			this.values = args[1] as int[];
		}

		public PropertyLayoutIntPopup(string label, SerializedProperty serializedProperty, int[] values)
		{
			this.Properties = new SerializedProperty[] { serializedProperty };
			this.Label = new GUIContent(label);
			this.options = values.Select(x => x.ToString()).ToArray();
			this.values = values;
		}

		public PropertyLayoutIntPopup(string label, SerializedProperty serializedProperty, string[] options, int[] values)
		{
			this.Properties = new SerializedProperty[] { serializedProperty };
			this.Label = new GUIContent(label);
			this.options = options;
			this.values = values;
		}

		public PropertyLayoutIntPopup(string label, SerializedObject serializedObject, string propertyName, string[] options, int[] values)
		{
			this.Properties = new SerializedProperty[] { serializedObject.FindProperty(propertyName) };
			this.Label = new GUIContent(label);
			this.options = options;
			this.values = values;
		}
	}
}