using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WeaponsSystem))]
public class WeaponsSystemEditor : ExtendedEditor
{
	// EditorStyles.toolbarButton
	WeaponsSystem _weaponsSystem;

	GUIStyle selectedStyle = new GUIStyle();
	GUIStyle idleStyle = new GUIStyle();

	private void OnEnable()
	{
		_weaponsSystem = (WeaponsSystem)target;
		selectedStyle.normal.background = MakeTex(2, 2, new Color(1f, 0f, 0f, 1f));
		idleStyle.normal.background = MakeTex(2, 2, new Color(0f, 1f, 0f, 1f));
	}

	bool _showDependencies = true;
	bool _showControlls = false;

	public override void OnInspectorGUI()
	{
		if (_weaponsSystem.useDefaultEditor)
		{
			base.OnInspectorGUI();
		}
		else
		{
			SerializedObject GunControllerObject = new SerializedObject(_weaponsSystem.GunController);

			EditorGUILayout.BeginVertical("box");

			GUILayout.Label("Ammo", EditorStyles.boldLabel);

			SerializedProperty ammoCount = GunControllerObject.FindProperty("ammoCount");
			SerializedProperty ammoArray = ammoCount.FindPropertyRelative("ammo");

			EditorGUILayout.BeginHorizontal();
		
			for (int index = 0; index < ammoArray.arraySize; index++) {
				EditorGUILayout.BeginVertical("box");

				if (index == 0) {
					GUILayout.Box("", selectedStyle, GUILayout.ExpandWidth(true), GUILayout.Height(8));
				}
				else {
					GUILayout.Box("", idleStyle, GUILayout.ExpandWidth(true), GUILayout.Height(8));
				}

				GUILayout.Box("", GUILayout.ExpandWidth(true),GUILayout.Height(50));
				GUILayout.Label(ammoArray.GetArrayElementAtIndex(index).intValue.ToString(), EditorStyles.centeredGreyMiniLabel, GUILayout.ExpandWidth(true));

				EditorGUILayout.EndVertical();
			}

			EditorGUILayout.EndHorizontal();

			EditorGUILayout.EndVertical();

			EditorGUILayout.BeginVertical("box");

			GUILayout.Label("Gun Controller", EditorStyles.boldLabel);

			DrawSettingEditor(_weaponsSystem.GunController, ref _weaponsSystem.showWeaponsSystem);

			EditorGUILayout.EndVertical();
		}

		serializedObject.ApplyModifiedProperties();

		DrawFootnote("Use Default Editor", ref _weaponsSystem.useDefaultEditor);
	}

	private Texture2D MakeTex(int width, int height, Color col)
	{
		Color[] pix = new Color[width * height];
		for (int i = 0; i < pix.Length; ++i)
		{
			pix[i] = col;
		}
		Texture2D result = new Texture2D(width, height);
		result.SetPixels(pix);
		result.Apply();
		return result;
	}
}