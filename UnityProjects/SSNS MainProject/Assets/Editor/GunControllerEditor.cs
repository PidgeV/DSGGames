using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GunController))]
public class GunControllerEditor : ExtendedEditor
{
	GunController _gunController;

	private void OnEnable()
	{
		if (target.GetType() == typeof(GunController))
		{
			_gunController = (GunController)target;
		}
	}

	public override void OnInspectorGUI()
	{
		if (_gunController != null)
		{
			GUILayout.Space(10);

			EditorGUILayout.BeginVertical("box");

			GUILayout.Label("Animators", EditorStyles.boldLabel);

			DrawField("gunAnimator");
			DrawField("swapAnimator");

			EditorGUILayout.EndVertical();

			EditorGUILayout.BeginVertical("box");

			GUILayout.Label("Projectiles", EditorStyles.boldLabel);

			DrawField("standardShot");
			DrawField("energyShot");
			DrawField("missileShot");
			DrawField("chargedShot");

			GUILayout.Space(5);

			DrawField("laserShot");

			EditorGUILayout.EndVertical();

			EditorGUILayout.BeginVertical("box");

			GUILayout.Label("Barrels", EditorStyles.boldLabel);

			DrawField("barrelL");
			DrawField("barrelR");

			EditorGUILayout.EndVertical();

			EditorGUILayout.BeginVertical("box");

			GUILayout.Label("Shield Projector", EditorStyles.boldLabel);

			DrawField("shieldProjector");

			EditorGUILayout.EndVertical();

			EditorGUILayout.BeginVertical("box");

			GUILayout.Label("Gun States", EditorStyles.boldLabel);

			DrawField("CanAttack");
			DrawField("CanSwap");

			EditorGUILayout.EndVertical();

			EditorGUILayout.BeginVertical("box");

			GUILayout.Label("Ammo", EditorStyles.boldLabel);

			DrawField("ammoCount");

			EditorGUILayout.EndVertical();

			serializedObject.ApplyModifiedProperties();
		}
	}
}
