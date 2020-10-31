﻿using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace Unitils
{
	[CustomPropertyDrawer(typeof(Enumeration), true)]
	public class EnumerationDrawer : PropertyDrawer
	{
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return EditorGUI.GetPropertyHeight(property, label, true);
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			SerializedProperty idProperty = property.FindPropertyRelative("id");
			SerializedProperty nameProperty = property.FindPropertyRelative("name");

			IEnumerable<Enumeration> enumeration = Enumeration.GetAll(this.fieldInfo.FieldType).OrderBy(x => x.Id);
			List<Enumeration> list = enumeration.ToList();

			int index = 0;
			Enumeration choosed = list.FirstOrDefault(x => x.Id == idProperty.intValue);
			if (choosed != null) index = Mathf.Max(list.IndexOf(choosed), 0);

			index = EditorGUI.Popup(position, label.text, index, list.Select(x => x.Name).ToArray());
			idProperty.intValue = list[index].Id;
			nameProperty.stringValue = list[index].Name;
		}
	}
}