using UnityEngine;
using UnityEditor;

namespace Unitils
{
	public class PropertyLayoutIntSlider : IPropertyLayout
	{
		private int leftValue;
		private int rightValue;

		public SerializedProperty[] Properties { get; private set; }
		public SerializedProperty Property => this.Properties[0];
		public GUIContent Label { get; private set; }

		public void Draw()
		{
			EditorGUILayout.IntSlider(this.Property, this.leftValue, this.rightValue, this.Label);
		}

		public void SetValues(params object[] args)
		{
			this.leftValue = (int)args[0];
			this.rightValue = (int)args[1];
		}

		public PropertyLayoutIntSlider(string label, SerializedProperty serializedProperty, int leftValue, int rightValue)
		{
			this.Properties = new SerializedProperty[] { serializedProperty };
			this.Label = new GUIContent(label);
			this.leftValue = leftValue;
			this.rightValue = rightValue;
		}

		public PropertyLayoutIntSlider(string label, SerializedObject serializedObject, string propertyName, int leftValue, int rightValue)
		{
			this.Properties = new SerializedProperty[] { serializedObject.FindProperty(propertyName) };
			this.Label = new GUIContent(label);
			this.leftValue = leftValue;
			this.rightValue = rightValue;
		}
	}
}