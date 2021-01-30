using UnityEngine;
using UnityEditor;

namespace Unitils
{
    [CustomEditor(typeof(ButtonSoundData))]
    public class ButtonSoundDataEditor : Editor
    {
		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

			ButtonSoundData data = this.target as ButtonSoundData;

			for (int i = 0; i < data.List.Count; i++) {
				ButtonSoundData.Pair pair = data.List[i];
				SerializedProperty pairProperty = this.serializedObject.FindProperty("list").GetArrayElementAtIndex(i);
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.PropertyField(pairProperty.FindPropertyRelative("soundName"), new GUIContent(pair.type.ToString()));
				EditorGUILayout.EndHorizontal();
			}

			this.serializedObject.ApplyModifiedProperties();
		}
	}
}