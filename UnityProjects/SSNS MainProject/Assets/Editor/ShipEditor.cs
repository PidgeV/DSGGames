using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ShipController))]
public class ShipEditor : ExtendedEditor
{
	// EditorStyles.toolbarButton
	ShipController _shipController;

	private void OnEnable()
	{
		_shipController = (ShipController)target;
	}

	bool _showDependencies = true;
	bool _showControlls = false;

	public override void OnInspectorGUI()
	{
		if (_shipController.useDefaultEditor)
		{
			base.OnInspectorGUI();
		}
		else
		{
			GUILayout.Space(10);

			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("Controlls", EditorStyles.toolbarButton))
			{
				_showDependencies = false;
				_showControlls = true;
			}
			if (GUILayout.Button("Dependencies", EditorStyles.toolbarButton))
			{
				_showDependencies = true;
				_showControlls = false;
			}
			EditorGUILayout.EndHorizontal();

			if (_showDependencies) {
				EditorGUILayout.BeginVertical("box");

				GUILayout.Label("Cameras", EditorStyles.boldLabel);

				DrawField("PilotCamera");
				DrawField("GunnerCamera");

				EditorGUILayout.EndVertical();

				EditorGUILayout.BeginVertical("box");

				GUILayout.Label("Transforms", EditorStyles.boldLabel);

				DrawField("ShipModel");

				GUILayout.Space(5);

				DrawField("gunnerPivot");
				DrawField("gunnerObject");

				GUILayout.Space(5);

				DrawField("gunnerslockUI");

				EditorGUILayout.EndVertical();

				EditorGUILayout.BeginVertical("box");

				GUILayout.Label("Audio", EditorStyles.boldLabel);

				DrawField("shootingSoundController");

				EditorGUILayout.EndVertical();

				EditorGUILayout.BeginVertical("box");

				GUILayout.Label("Warp Effect", EditorStyles.boldLabel);

				DrawField("warpEffect1");
				DrawField("warpEffect2");

				EditorGUILayout.EndVertical();
			}

			if (_showControlls) {
				EditorGUILayout.BeginVertical("box");

				GUILayout.Label("Control Options", EditorStyles.boldLabel);

				DrawField("invertedControls");
				DrawField("unlimitedBoost");
				DrawField("lockOn");

				EditorGUILayout.EndVertical();

				EditorGUILayout.BeginVertical("box");

				GUILayout.Label("Behaviour", EditorStyles.boldLabel);

				DrawField("myBehaviour");
				DrawSettingEditor(_shipController.myBehaviour, ref _shipController.showBehaviour);

				EditorGUILayout.EndVertical();

				EditorGUILayout.BeginVertical("box");

				GUILayout.Label("Stats", EditorStyles.boldLabel);

				DrawField("myStats");
				DrawSettingEditor(_shipController.myStats,ref _shipController.showStats);
				EditorGUILayout.EndVertical();
			}

			serializedObject.ApplyModifiedProperties();
		}

		DrawFootnote("Use Default Editor", ref _shipController.useDefaultEditor);
	}
}