using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// NOTE -- This script shoud not be on anything by default
//           It is given to an Input Object after the player selects their role

public class ShipInput : MonoBehaviour
{
	public PilotController controller;

	// The last input -- This is the same as Input.GetAxis();
	public Vector2 move = new Vector2();

	// If you want to know if a button is being held down 
	// You can reference these
	public bool RotateLeft = false;
	public bool RotateRight = false;
	public bool MapToggle = false;
	public bool Boosting = false;
	public bool JobSwap = false;
	public bool Break = false;

	// DEBUG
	public bool printDebug = false;

	private void Update()
	{
		if (controller)
		{
			controller.Move(move);
			controller.Boost(Boosting);
			controller.SetShipTransfrom(move, Boosting);
		}
	}

	// You NEED to give this script a PilotController for it to update anything
	public void GiveController(PilotController newPilotController)
	{
		controller = newPilotController;
	}

	// IF YOU WANT CODE TO ACTIVATE ON A BUTTON CLICK USE THESE

	// NOTE -- These are the input functions called AUTOMATICALLY by the Player Input Script 
	//		   [Part of unitys new input system]

	#region Input Functions

	void OnHorizontal(InputValue value)
	{
		move.y = -value.Get<float>();
	}

	void OnVertical(InputValue value)
	{
		move.x = -value.Get<float>();
	}

	void OnMove(InputValue value)
	{
		move = value.Get<Vector2>();

		if (printDebug) { Helper.PrintTime("Ship -- OnMove" + "[ " + move + " ]"); }
	}

	void OnBoost(InputValue value)
	{
		Boosting = value.Get<float>() <= 0.5f ? false : true;

		if (printDebug) { Helper.PrintTime("Ship -- OnBoost" + " [" + Boosting + "]"); }
	}

	void OnJobSwap(InputValue value)
	{
		JobSwap = value.Get<float>() <= 0.5f ? false : true;

		if (printDebug) { Helper.PrintTime("Ship -- OnJobSwap" + "[ " + JobSwap + " ]"); }
	}

	void OnMapToggle(InputValue value)
	{
		MapToggle = value.Get<float>() <= 0.5f ? false : true;

		if (printDebug) { Helper.PrintTime("Ship -- OnMapToggle" + "[ " + MapToggle + " ]"); }
	}

	void OnRotateLeft(InputValue value)
	{
		RotateLeft = value.Get<float>() <= 0.5f ? false : true;

		if (printDebug) { Helper.PrintTime("Ship -- OnRotateLeft" + "[ " + RotateLeft + " ]"); }
	}

	void OnRotateRight(InputValue value)
	{
		RotateRight = value.Get<float>() <= 0.5f ? false : true;

		if (printDebug) { Helper.PrintTime("Ship -- OnRotateRight" + "[ " + RotateRight + " ]"); }
	}

	void OnBreak(InputValue value)
	{
		Break = value.Get<float>() <= 0.5f ? false : true;

		if (printDebug) { Helper.PrintTime("Ship -- OnBreak" + "[ " + Break + " ]"); }
	}
	#endregion
}
