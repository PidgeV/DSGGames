using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretController : MonoBehaviour
{
	private Vector3 turretRotation = new Vector3();

	public float speed = 75f;

	public bool canShoot = true;
	public List<Barrel> barrels;

	private bool lockToShipRotation = true;

	public void Move(Vector2 move)
	{
		Vector3 newMove = new Vector3(-move.y, move.x, 0.0f) * speed * Time.deltaTime;

		// Add the input to our current ships rotation
		// The reason I have a vector3 for it is so its consistent
		turretRotation += newMove;

		// Apply the rotation and move the shop forward
		transform.localRotation = Quaternion.Euler(turretRotation.x, turretRotation.y, 0.0f);

		// Rotate the turret
		if (!lockToShipRotation)
		{
			// If we use localRotation it locks the rotation to the ship
			transform.localRotation = Quaternion.Euler(turretRotation.x, turretRotation.y, 0.0f);
		}
		else
		{
			// If we use normal rotation it lets the turret rotate independently
			transform.rotation = Quaternion.Euler(turretRotation.x, turretRotation.y, 0.0f);
		}

        //Prevent ship from looking up  - Cant test so will need work
        float dot = Vector3.Dot(transform.forward, transform.parent.transform.up);

        if (dot < 0)
        {
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, 0, 0);
        }
    }

	public void Shoot()
	{
		if (canShoot)
		{
			// Shoot
			foreach (Barrel barrel in barrels)
				barrel.Shoot();
		}
	}
}
