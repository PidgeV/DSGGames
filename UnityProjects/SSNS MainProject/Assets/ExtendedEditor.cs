using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class ExtendedEditor : Editor
{
	protected void DrawSettingEditor(Object settings, ref bool foldout)
	{
		if (settings != null)
		{
			EditorGUILayout.BeginVertical("box");
			foldout = EditorGUILayout.InspectorTitlebar(foldout, settings);
			if (foldout)
			{
				Editor editor = Editor.CreateEditor(settings);
				if (editor) { editor.OnInspectorGUI(); }
			}
			EditorGUILayout.EndVertical();
		}
	}

	protected void DrawField(string propName)
	{
		EditorGUILayout.PropertyField(serializedObject.FindProperty(propName), true);
	}

	protected void DrawFootnote(string msg, ref bool showDefault)
	{
		EditorGUILayout.BeginVertical("box");
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.HelpBox(msg, MessageType.Info);
		if (GUILayout.Button("Editor", GUILayout.Width(80), GUILayout.Height(38)))
		{
			showDefault = !showDefault;
		}
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.EndVertical();
	}
}
