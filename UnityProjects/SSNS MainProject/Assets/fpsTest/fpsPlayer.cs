using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class fpsPlayer : MonoBehaviour
{
	public float speed = 15f;

	Vector2 move;
	Vector2 rotate;

	public float shoot = 0;

	public Barrel gun;

	// Update is called once per frame
	void Update()
	{
		transform.position += new Vector3(move.x, 0, move.y);
		transform.LookAt(transform.position + new Vector3(rotate.x, 0, rotate.y));

		if (shoot > 0.5f)
		{
			gun.Shoot();
		}
	}

	void OnMove(InputValue value)
	{
		move = value.Get<Vector2>() * Time.deltaTime * speed;
	}

	void OnRotate(InputValue value)
	{
		rotate = -value.Get<Vector2>();
	}

	void OnShoot(InputValue value)
	{
		shoot = value.Get<float>();
	}
}
