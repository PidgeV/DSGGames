﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// NOTE -- Any gameObjects with a Bullet script should also have a Damage script
//		   The Damage script holds the damage members for any collidables including bullets

// The Bullet Class keeps track of how long a projectile should stay alive and how / how fast it should move
public class Bullet : MonoBehaviour
{
	// The time the projectile is alive after its Instantiated
	public float lifetime = 0.8f;

	// The speed the projectile moves
	public float speed = 50.0f;

	// A counter till the projectile will be destroyed
	float counter = 0.0f;

	// Update is called once per frame
	void Update()
	{
		// Move the projectile forward
		transform.position += transform.forward * Time.deltaTime * speed;

		// Check if the projectile has to be destroyed
		if (counter > lifetime)
		{
			Destroy(this.gameObject);
		}
		else
		{
			counter += Time.deltaTime;
		}
	}
}
