using UnityEngine;
using UnityEditor;
using UnityEditor.UI;

namespace Unitils
{
	[CustomEditor(typeof(AtlasImage))]
	[CanEditMultipleObjects]
	public class AtlasImageEditor : ImageEditor
	{
		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

			EditorGUILayout.BeginVertical();

			EditorGUILayout.PropertyField(this.serializedObject.FindProperty("atlas"), new GUIContent("Sprite Atlas"));
			EditorGUILayout.PropertyField(this.serializedObject.FindProperty("spriteName"), new GUIContent("Sprite Name"));

			AtlasImage atlasImage = this.target as AtlasImage;

			if (atlasImage.Atlas != null) {
				if (GUILayout.Button("Selection") && atlasImage.Atlas != null) {
					AtlasImageSelectionWindow.Open(
						atlasImage.Atlas,
						null,
						spriteName => atlasImage.SpriteName = spriteName
					);
				}
				EditorGUILayout.Space();
			}

			EditorGUILayout.EndVertical();

			this.serializedObject.ApplyModifiedProperties();

			base.OnInspectorGUI();
		}
	}
}