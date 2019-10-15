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
	[Space(10)]

	#region Ship Stats
	[Tooltip("The Speed the ship flys")]
	public float speed = 20.0f;
	public float boost = 2.0f;

	[Tooltip("The speed you can turn left right up and down")]
	public float rotationSpeed = 20.0f;

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

	public void Move(Vector2 move)
	{
		Vector3 newMove = new Vector3(-move.y, move.x, 0.0f) * rotationSpeed;

		// Add the input to our current ships rotation
		// The reason I have a vector3 for it is so its consistent
		shipRotation += newMove;

		// Apply the rotation and move the shop forward
		transform.localRotation = Quaternion.Euler(shipRotation.x, shipRotation.y, 0.0f);
	}

	public void Boost(bool boosting)
	{
		// MOVE the ship forward
		if (boosting)
		{
			// If we ARE boosting
			transform.Translate(Vector3.forward * Time.deltaTime * speed * boost);
		}
		else
		{
			// If we are NOT boosting
			transform.Translate(Vector3.forward * Time.deltaTime * speed);
		}
	}

	// This updates the ship model
	public void SetShipTransfrom(Vector2 move, bool boost )
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
			// ship.transform.localRotation = Quaternion.Lerp(ship.transform.localRotation, Quaternion.Euler(0, 0, new Quaternion(0f, 0f, dialManager.GetRotation(), 1f).z), 0.05f);
			ship.transform.localRotation = Quaternion.Lerp(ship.transform.localRotation, Quaternion.identity, 0.05f);
			ship.transform.localPosition = Vector3.Lerp(ship.transform.localPosition, Vector3.zero, 0.02f);
		}
		else
		{
			Debug.LogError("Why is there no ship?");
		}
	}
}
