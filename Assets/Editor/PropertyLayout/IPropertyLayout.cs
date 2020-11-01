using UnityEngine;
using UnityEditor;

namespace Unitils
{
	public interface IPropertyLayout
	{
		SerializedProperty[] Properties { get; }
		SerializedProperty Property { get; }
		GUIContent Label { get; }
		void Draw();
		void SetValues(params object[] args);
	}
}