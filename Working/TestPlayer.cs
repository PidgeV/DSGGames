using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

public class TestPlayer : MonoBehaviour
{
	Rigidbody rigidbody;
	
	public float speed = 15f;

	public Vector3 rotationSpeed = new Vector3();

	// Start is called before the first frame update
	void Start()
	{
		rigidbody = GetComponent<Rigidbody>();	
	}

	// Update is called once per frame
	void Update()
	{
		rigidbody.velocity = transform.forward * Time.deltaTime * speed * 100f;

		// Get the players INPUT
		float inputX = Input.GetAxis("Horizontal");
		float inputY = Input.GetAxis("Vertical");
		//float inputZ = Input.GetAxis("Spin");

		// GET the rotation speed to moving UP and DOWN
		float xRot = -inputY * Time.deltaTime * 10f * rotationSpeed.y;

		// GET the rotation speed to moving LEFT and RIGHT
		float yRot = inputX * Time.deltaTime * 10f * rotationSpeed.x;

		// GET the rotation speed to SPIN
		//float zRot = -inputZ * Time.deltaTime * 10f * rotationSpeed.z;

		// Set the new ROTATION of the ship
		transform.localRotation = transform.localRotation * Quaternion.Euler(xRot, yRot, 0);
	}
}
