using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// NOTE -- This script shoud not be on anything by default
//		   It is given to an Input Object after the player selects their role

public class ShipInput : MonoBehaviour
{
	public PilotController pilotController;

	Vector3 lastInput = new Vector3();

	bool boosting = false;

	private void Awake()
	{
		pilotController = FindObjectOfType<PilotController>();
	}

	private void Update()
	{
		pilotController.Move(lastInput * Time.deltaTime);
		pilotController.Boost(boosting);
		pilotController.SetShipTransfrom(lastInput * Time.deltaTime, boosting);
	}

	void OnMove(InputValue value)
	{
		lastInput = value.Get<Vector2>();
	}

	void OnBoost(InputValue value)
	{
		boosting = value.Get<float>() <= 0.5f ? false : true;
	}

	void OnJobSwap()
	{
	}

	void OnMapToggle()
	{
	}

	void OnRotateLeft()
	{
	}

	void OnRotateRight()
	{
	}

	void OnBreak()
	{
	}
}
