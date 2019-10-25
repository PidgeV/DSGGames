using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// NOTE -- This script shoud not be on anything by default
//           It is given to an Input Object after the player selects their role

/// <summary>
/// The Ship Input script controls inputs for a Pilot Controller
/// </summary>
public class ShipInput : Controller
{
	public PilotController controller;

	// The last input -- This is the same as Input.GetAxis();
	Vector2 move = new Vector2();

	// If you want to know if a button is being held down 
	// You can reference these
	public bool MapToggle = false;
	public bool Boosting = false;
	public bool JobSwap = false;
	public bool Break = false;
	
	private void Update()
	{
		if (MenuMode)
		{
			// Increment the timer
			menuCounter += Time.deltaTime;

			if (menuCounter > menuChangeTime)
			{
				// Reset the counter
				menuCounter = 0f;

				SendMenuInput(move);
			}
		}
		else
		{
			if (controller)
			{
				controller.Move(move);
				controller.Boost(Boosting);
				controller.SetShipTransfrom(move, Boosting);
			}
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

	void OnKeyboardMove(InputValue value)
	{
		move = value.Get<Vector2>();

		#region Debug

		if (printDebug) { Helper.PrintTime("Ship -- OnKeyboardMove" + "[ " + move + " ]"); }

		#endregion
	}

	void OnMove(InputValue value)
	{
		move = value.Get<Vector2>();

		#region Debug

		if (printDebug) { Helper.PrintTime("Ship -- OnMove" + "[ " + move + " ]"); }

		#endregion
	}

	void OnBoost(InputValue value)
	{
		Boosting = value.isPressed;

		#region Debug

		if (printDebug) { Helper.PrintTime("Ship -- OnBoost" + " [" + Boosting + "]"); }

		#endregion
	}

	void OnJobSwap(InputValue value)
	{
		JobSwap = value.isPressed;

		#region Debug

		if (printDebug) { Helper.PrintTime("Ship -- OnJobSwap" + "[ " + JobSwap + " ]"); }

		#endregion
	}

	void OnMapToggle(InputValue value)
	{
		MapToggle = value.isPressed;

		#region Debug

		if (printDebug) { Helper.PrintTime("Ship -- OnMapToggle" + "[ " + MapToggle + " ]"); }

		#endregion
	}

	void OnBreak(InputValue value)
	{
		Break = value.isPressed;

		#region Debug

		if (printDebug) { Helper.PrintTime("Ship -- OnBreak" + "[ " + Break + " ]"); }

		#endregion
	}

	void OnRotateLeft(InputValue value)
	{
		#region Debug

		if (printDebug) { Helper.PrintTime("Ship -- OnBreak" + "[ " + Break + " ]"); }

		#endregion
	}

	void OnRotateRight(InputValue value)
	{
		#region Debug

		if (printDebug) { Helper.PrintTime("Ship -- OnBreak" + "[ " + Break + " ]"); }

		#endregion
	}

	#endregion
}
