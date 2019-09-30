using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public GameObject Reticle;
	public GameObject camera;
	public GameObject Ship;

	[Header("Speed")]

	[Range(0, 1)]
	public float speed = 0.5f;
	public float boostMultiplier = 3f;

	public float reticalRange = 6;

	float cameraMoveSpeed = 0.2f;
	float cameraLookSpeed = 2;

	[Header("Turn")]
	public Vector2 turnSpeed = new Vector2(15, 15);

	[Header("Retical")]
	public Vector2 radicalSize = new Vector2(15, 15);
	public Vector2 radicalSpeed = new Vector2(9, 6);

	private void FixedUpdate()
	{
		float inputX = Input.GetAxis("Horizontal") * Time.deltaTime * radicalSpeed.x;
		float inputY = Input.GetAxis("Vertical") * Time.deltaTime * radicalSpeed.y;

		// Move the RETICLE relative to the camera
		Vector3 reticlePos = Reticle.transform.localPosition += new Vector3(inputX, inputY, 0);

		// CLAMP the reticle to the screen
		reticlePos.x = Mathf.Clamp(reticlePos.x, -radicalSize.x, radicalSize.x);
		reticlePos.y = Mathf.Clamp(reticlePos.y, -radicalSize.y, radicalSize.y);

		// SET the reticle position
		Reticle.transform.localPosition = Vector3.Lerp(reticlePos, new Vector3(0, 0, reticalRange), 0.1f);

		float shipSpeed = speed;

		float xRot = -Reticle.transform.localPosition.y * Time.deltaTime * turnSpeed.x;
		float yRot = Reticle.transform.localPosition.x * Time.deltaTime * turnSpeed.y;

		if (Input.GetKey(KeyCode.LeftShift))
		{
			Ship.transform.rotation = Quaternion.Lerp(Ship.transform.localRotation, Quaternion.Euler(0, Ship.transform.rotation.eulerAngles.y, 0), 0.01f);
		}
		else
		{
			Ship.transform.Rotate(new Vector3(xRot, yRot, 0));
		}

		// GET the position the camera should be at
		Vector3 newCameraPosition = Ship.transform.position + -Ship.transform.forward * 5f + new Vector3(0, 1.5f, 0);

		// UPDATE the CAMERAS POSITION 
		camera.transform.position = Vector3.Lerp(camera.transform.position, newCameraPosition, cameraMoveSpeed);

		// UPDATE the CAMERAS ROTATION
		camera.transform.localRotation = Quaternion.Lerp(camera.transform.localRotation, Ship.transform.localRotation, cameraMoveSpeed / cameraLookSpeed);

		// MOVE the SHIP
		if (Input.GetKey(KeyCode.Space))
		{
			shipSpeed *= boostMultiplier;
		}

		// MOVE the ship
		Ship.transform.Translate(new Vector3(0, 0, shipSpeed));
	}
}
