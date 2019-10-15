using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A barrel fires a projectile
public class Barrel : MonoBehaviour
{
	// The speed the barrel can instantiate projectiles
	public float fireRate = 0.1f;

	// A counter till the next projectile can be fired
	float counter = 0.0f;

	// The Type of projectile to fire
	public Bullet type;

	// Update is called once per frame
	void Update()
	{
		// Check if a projectile can be fired
		if (counter > fireRate)
		{
			if (Input.GetAxis("Fire1") > 0)
			{
				Instantiate(type, transform.position, transform.rotation);
				counter = 0;
			}
		}
		else
		{
			counter += Time.deltaTime;
		}
	}
}
