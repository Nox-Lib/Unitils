using UnityEngine;
using UnityEditor;

namespace Unitils
{
	public class PropertyLayoutMinMaxSlider : IPropertyLayout
	{
		private float minLimit;
		private float maxLimit;
		private bool isIntSnap;

		public SerializedProperty[] Properties { get; }
		public SerializedProperty Property => null;
		public GUIContent Label { get; }

		public void Draw()
		{
			float min = this.isIntSnap ? this.Properties[0].intValue : this.Properties[0].floatValue;
			float max = this.isIntSnap ? this.Properties[1].intValue : this.Properties[1].floatValue;

			EditorGUILayout.MinMaxSlider(this.Label, ref min, ref max, this.minLimit, this.maxLimit);

			if (this.isIntSnap) {
				this.Properties[0].intValue = Mathf.RoundToInt(min);
				this.Properties[1].intValue = Mathf.RoundToInt(max);
			}
			else {
				this.Properties[0].floatValue = min;
				this.Properties[1].floatValue = max;
			}
		}

		public void SetValues(params object[] args) {}

		public PropertyLayoutMinMaxSlider(string label, SerializedProperty minProperty, SerializedProperty maxProperty, int minLimit, int maxLimit, bool isIntSnap)
		{
			this.Properties = new SerializedProperty[] { minProperty, maxProperty };
			this.Label = new GUIContent(label);
			this.minLimit = minLimit;
			this.maxLimit = maxLimit;
			this.isIntSnap = isIntSnap;
		}

		public PropertyLayoutMinMaxSlider(string label, SerializedObject serializedObject, string minPropertyName, string maxPropertyName, int minLimit, int maxLimit, bool isIntSnap)
		{
			this.Properties = new SerializedProperty[] { serializedObject.FindProperty(minPropertyName), serializedObject.FindProperty(maxPropertyName) };
			this.Label = new GUIContent(label);
			this.minLimit = minLimit;
			this.maxLimit = maxLimit;
			this.isIntSnap = isIntSnap;
		}
	}
}