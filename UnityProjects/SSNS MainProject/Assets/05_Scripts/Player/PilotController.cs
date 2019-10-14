using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PilotController : MonoBehaviour
{
	// This holds the added rotation from inputs
	private Vector3 shipRotation = new Vector3();
	private float currentRotation = 0.0f;

	[Tooltip("A reference to the ship gameobject")]
	public GameObject ship;
	public GameObject gunner;
	[Space(10)]

	#region Ship Stats
	[Tooltip("The Speed the ship flys")]
	public float speed = 20.0f;
	public float boost = 2.0f;

	[Tooltip("The speed you can turn left right up and down")]
	public float rotationSpeed = 50.0f;

	[Space(10)]
	[Tooltip("This is the speed the ship model turns")]
	public Vector2 simulatedRotation = new Vector2(70.0f, 50.0f);
	public float simulatedBoost = 0.2f;
	#endregion

	// The Dial Manager
	[Space(10)]
	public Dial dialManager;

	// Start is called before the first frame update
	void Start()
	{
		if (!dialManager)
		{
			Debug.LogError("Why is there no Dial Manager?");
		}
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		// GET INPUTS
		float x = Input.GetAxis("JHorizontal") * Time.deltaTime * rotationSpeed;
		float y = Input.GetAxis("JVertical") * Time.deltaTime * rotationSpeed;

		Vector3 newMove = new Vector3(-y, x, 0.0f);
		//Vector3 newMove = new Vector3(0.1f, 0.1f, 0.0f);

		// Add the input to our current ships rotation
		// The reason I have a vector3 for it is so its consistent
		shipRotation += newMove;

		// Apply the rotation and move the shop forward
		transform.localRotation = Quaternion.Euler(shipRotation.x, shipRotation.y, 0.0f);

		// MOVE the ship forward
		if (Input.GetAxis("Fire2") > 0)
		{
			// If we ARE boosting
			transform.Translate(Vector3.forward * Time.deltaTime * speed * boost);
			SetShipTransfrom(true);
		}
		else
		{
			// If we are NOT boosting
			transform.Translate(Vector3.forward * Time.deltaTime * speed);
			SetShipTransfrom();
		}
	}

	private void LateUpdate()
	{
		// Spin
		SickoMode();
	}

	// This updates the ship model
	void SetShipTransfrom(bool boost = false)
	{
		if (ship)
		{

			float x = Input.GetAxis("JHorizontal") * Time.deltaTime * simulatedRotation.x;
			float y = Input.GetAxis("JVertical") * Time.deltaTime * simulatedRotation.y;

			// MOVE the ship model
			ship.transform.Rotate(-y, 0.0f, -x);

			// BOOST the ship forward
			if (boost)
			{
				ship.transform.position = ship.transform.position + (transform.forward * Time.deltaTime * simulatedBoost);
			}

			// Slowly move the ship back to its initial position
			//ship.transform.localRotation = Quaternion.Lerp(ship.transform.localRotation, Quaternion.Euler(0, 0, new Quaternion(0f, 0f, dialManager.GetRotation(), 1f).z), 0.05f);
			ship.transform.localRotation = Quaternion.Lerp(ship.transform.localRotation, Quaternion.identity, 0.05f);
			ship.transform.localPosition = Vector3.Lerp(ship.transform.localPosition, Vector3.zero, 0.02f);
		}
		else
		{
			Debug.LogError("Why is there no ship?");
		}
	}

	void SickoMode()
	{
		//// Im so sorry
		//if (Input.GetAxis("BumperR") > 0 &&
		//	Input.GetAxis("BumperL") > 0 &&
		//	Input.GetAxis("Fire1") > 0 &&
		//	Input.GetAxis("Fire2") > 0)
		//{
		//	Debug.Log("REEEEEEEE");
		//	gunner.transform.Rotate(257, -155, 205);
		//}
	}
}
