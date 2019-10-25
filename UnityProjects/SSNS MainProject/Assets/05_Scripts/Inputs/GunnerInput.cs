using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// NOTE -- This script shoud not be on anything by default
//           It is given to an Input Object after the player selects their role

/// <summary>
/// The Gunner Input script controls inputs for a Gunner Controller
/// </summary>
public class GunnerInput : Controller
{
	public TurretController controller;

	// The last input -- This is the same as Input.GetAxis();
	Vector2 move = new Vector2();
	Vector2 weaponSwap = new Vector2();

	// If you want to know if a button is being held down 
	// You can reference these
	public bool ChangeWeapons = false;
	public bool MapToggle = false;
	public bool JobSwap = false;
	public bool Shoot = false;

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
			}
		}
	}

	// You NEED to give this script a TurretController for it to update anything
	public void GiveController(TurretController newTurretController)
	{
		controller = newTurretController;
	}

	// IF YOU WANT CODE TO ACTIVATE ON A BUTTON CLICK USE THESE

	// NOTE -- These are the input functions called AUTOMATICALLY by the Player Input Script 
	//		   [Part of unitys new input system]

	#region Input Functions

	void OnMove(InputValue value)
	{
		move = value.Get<Vector2>();

		#region Debug

		if (printDebug) { Helper.PrintTime("Gunner -- OnMove" + "[ " + move + " ]"); }

		#endregion
	}

	void OnJobSwap(InputValue value)
	{
		JobSwap = value.isPressed;

		#region Debug

		if (printDebug) { Helper.PrintTime("Gunner -- OnJobSwap" + "[ " + JobSwap + " ]"); }

		#endregion
	}

	void OnMapToggle(InputValue value)
	{
		MapToggle = value.isPressed;

		#region Debug
		if (printDebug) { Helper.PrintTime("Gunner -- OnMapToggle" + "[ " + MapToggle + " ]"); }
		#endregion
	}

	void OnShoot(InputValue value)
	{
		Shoot = value.isPressed;

		#region Debug

		if (printDebug) { Helper.PrintTime("Gunner -- OnShoot" + "[ " + Shoot + " ]"); }

		#endregion
	}

	void OnChangeWeapons(InputValue value)
	{
		weaponSwap = value.Get<Vector2>();

		#region Debug

		if (printDebug) { Helper.PrintTime("Gunner -- OnChangeWeapons" + "[ " + weaponSwap + " ]"); }

		#endregion
	}

	#endregion
}
