using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// The IdleInputs is the default state for Controllers / Inputs 
// When a new Controller connects it is given the type of IdleInputs

/// <summary>
///  The Player has no job
/// </summary>
public class IdleInputs : Controller
{
	// The last input -- This is the same as Input.GetAxis();
	Vector2 lastInput = new Vector2();

	// Start is called before the first frame update
	private void Start()
	{
		// Join the game
		JoinGame();

		MenuMode = true;
	}

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

				SendMenuInput(lastInput);
			}
		}
		else
		{
			// Else if we're doing something outside of a menu
		}
	}

	// IF YOU WANT CODE TO ACTIVATE ON A BUTTON CLICK USE THESE

	// NOTE -- These are the input functions called AUTOMATICALLY by the Player Input Script 
	//		   [Part of unitys new input system]

	#region Input Functions

	void OnMove(InputValue value)
	{
		lastInput = value.Get<Vector2>();

		#region Debug

		if (printDebug) { Helper.PrintTime("Controller -- OnMove" + "[ " + lastInput + " ]"); }

		#endregion
	}

	void OnEnter(InputValue value)
	{
		if (value.isPressed)
		{
			UIManager.Instance.Enter();
		}

		#region Debug

		if (printDebug) { Helper.PrintTime("Ship -- OnEnter" + " [" + value.isPressed + "]"); }

		#endregion
	}

	void OnBack(InputValue value)
	{
		if (value.isPressed)
		{
			UIManager.Instance.ReturnToLastMenu();
		}

		#region Debug

		if (printDebug) { Helper.PrintTime("Ship -- OnBack" + " [" + value.isPressed + "]"); }

		#endregion
	}

	#endregion

	// This is used to auto change to gunner or a pilot
	// This is used for debugging the roles and inputs currency
	// [WILL BE REMOVED]
	#region Testing Controller Jobs

	// CONTROLLER
	// Removes the NoJobInputs and gives the player the PlayerInput 
	void OnConvertToPiolet()
	{
		Debug.Log("Converting.. [ Pilot Inputs Activated ]");

		gameObject.AddComponent<ShipInput>();

		// THIS IS A PLACEHOLDER FRO NOW -- FIND A BETTER WAY OF SETTING THE SHIPS 
		gameObject.GetComponent<ShipInput>().GiveController(FindObjectOfType<PilotController>());
		gameObject.GetComponent<ShipInput>().PlayerData = this.PlayerData;

		gameObject.GetComponent<PlayerInput>().defaultActionMap = "Ship";
		gameObject.GetComponent<PlayerInput>().SwitchCurrentActionMap("Ship");

		Destroy(this);
	}

	// KAYBOARD
	// Removes the NoJobInputs and gives the player the PlayerInput 
	void OnConvertKeyboard()
	{
		Debug.Log("Converting.. [ Pilot Inputs Activated ]");

		gameObject.AddComponent<ShipInput>();

		// THIS IS A PLACEHOLDER FRO NOW -- FIND A BETTER WAY OF SETTING THE SHIPS 
		gameObject.GetComponent<ShipInput>().GiveController(FindObjectOfType<PilotController>());
		gameObject.GetComponent<ShipInput>().PlayerData = this.PlayerData;

		gameObject.GetComponent<PlayerInput>().defaultActionMap = "Ship";
		gameObject.GetComponent<PlayerInput>().SwitchCurrentActionMap("Ship");

		Destroy(this);
	}

	// Removes the NoJobInputs and gives the player the GunnerInputs 
	void OnConvertToGunner()
	{
		Debug.Log("Converting.. [ Gunner Inputs Activated ]");

		gameObject.AddComponent<GunnerInput>();

		// THIS IS A PLACEHOLDER FRO NOW -- FIND A BETTER WAY OF SETTING THE GUNNER 
		gameObject.GetComponent<GunnerInput>().GiveController(FindObjectOfType<TurretController>());
		gameObject.GetComponent<GunnerInput>().PlayerData = this.PlayerData;

		gameObject.GetComponent<PlayerInput>().defaultActionMap = "Gunner";
		gameObject.GetComponent<PlayerInput>().SwitchCurrentActionMap("Gunner");

		Destroy(this);
	}

	#endregion
}
