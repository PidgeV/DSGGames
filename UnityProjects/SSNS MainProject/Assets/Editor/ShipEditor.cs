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
	bool _showAuto = false;

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
				_showAuto = false;
				_showDependencies = false;
				_showControlls = true;
			}
			if (GUILayout.Button("Dependencies", EditorStyles.toolbarButton))
			{
				_showAuto = false;
				_showDependencies = true;
				_showControlls = false;
			}
			if (GUILayout.Button("Auto", EditorStyles.toolbarButton))
			{
				_showAuto = true;
				_showDependencies = false;
				_showControlls = false;
			}
			EditorGUILayout.EndHorizontal();

			if (_showDependencies)
			{
				EditorGUILayout.BeginVertical("box");

				GUILayout.Label("Cameras", EditorStyles.boldLabel);

				DrawField("pilotCamera");
				DrawField("gunnerCamera");

				EditorGUILayout.EndVertical();

				EditorGUILayout.BeginVertical("box");

				GUILayout.Label("Transforms", EditorStyles.boldLabel);

				DrawField("shipModel");

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

			if (_showAuto)
			{
				EditorGUILayout.BeginVertical("box");

				GUILayout.Label("Auto", EditorStyles.boldLabel);

				DrawField("shieldProjector");
				DrawField("weaponsSystem");
				DrawField("playerHUD");
				DrawField("rigidbody");
				DrawField("shipHP");

				EditorGUILayout.EndVertical();

				EditorGUILayout.BeginVertical("box");

				GUILayout.Label("Lock On Target", EditorStyles.boldLabel);

				DrawField("lockOnTarget");

				EditorGUILayout.EndVertical();
			}

			if (_showControlls)
			{
				EditorGUILayout.BeginVertical("box");

				GUILayout.Label("Control Options", EditorStyles.boldLabel);

				DrawField("InvertedControls");
				DrawField("UnlimitedBoost");
				DrawField("AimAssist");

				EditorGUILayout.EndVertical();

				EditorGUILayout.BeginVertical("box");

				GUILayout.Label("Properties", EditorStyles.boldLabel);

				DrawField("myProperties");
				DrawSettingEditor(_shipController.Properties, ref _shipController.showStats);
				EditorGUILayout.EndVertical();

				EditorGUILayout.BeginVertical("box");

				GUILayout.Label("Behaviour", EditorStyles.boldLabel);

				DrawField("myBehaviour");
				DrawSettingEditor(_shipController.Behaviour, ref _shipController.showBehaviour);

				EditorGUILayout.EndVertical();
			}

			serializedObject.ApplyModifiedProperties();
		}

		DrawFootnote("Use Default Editor", ref _shipController.useDefaultEditor);
	}
}