using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretController : MonoBehaviour
{
	private Vector3 turretRotation = new Vector3();

	public float speed = 75f;

	public bool lockToShipRotation = false;

	// Update is called once per frame
	void Update()
	{
		// GET the input
		float x = Input.GetAxis("JHorizontal2") * Time.deltaTime * speed;
		float y = Input.GetAxis("JVertical2") * Time.deltaTime * speed;

		// ADD the input to the rotation
		turretRotation += new Vector3(-y, x, 0.0f);

		// Clamp the turrets rotation
		//turretRotation.x = Mathf.Clamp(turretRotation.x, -15, 80);

		// Rotate the turret
		if (lockToShipRotation)
		{
			// If we use localRotation it locks the rotation to the ship
			transform.localRotation = Quaternion.Euler(turretRotation.x, turretRotation.y, 0.0f);
		}
		else
		{
			// If we use normal rotation it lets the turret rotate independently
			transform.rotation = Quaternion.Euler(turretRotation.x, turretRotation.y, 0.0f);
		}
	}
}
