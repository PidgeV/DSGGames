using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// The NoJobInputs is the default class for inputs 
// When a new controller connects it is of type NoJobInputs

// [WILL BE REMOVED]
// Click right on the d-pad to become a Gunner
// Click left on the d-pad to become a Piolet 

public class NoJobInputs : MonoBehaviour
{
	// The last input -- This is the same as Input.GetAxis();
	public Vector2 move = new Vector2();

	// If you want to know if a button is being held down 
	// You can reference these
	public bool Enter = false;
	public bool Back = false;

	// DEBUG
	public bool printDebug = false;

	private void Start()
	{
	}

	private void Update()
	{
	}

	// IF YOU WANT CODE TO ACTIVATE ON A BUTTON CLICK USE THESE

	// NOTE -- These are the input functions called AUTOMATICALLY by the Player Input Script 
	//		   [Part of unitys new input system]

	#region Input Functions

	void OnMove(InputValue value)
	{
		Vector2 input = value.Get<Vector2>();

		if (input.x < -0.5f)
		{
			Helper.PrintTime("Left");
		}
		else if (input.x > 0.5f)
		{
			Helper.PrintTime("Right");
		}
		else if (input.y > 0.5f)
		{
			Helper.PrintTime("Up");
		}
		else if (input.y < -0.5f)
		{
			Helper.PrintTime("Down");
		}

		#region Debug

		if (printDebug) { Helper.PrintTime("Controller -- OnMove" + "[ " + move + " ]"); }

		#endregion
	}

	void OnEnter(InputValue value)
	{
		Enter = value.isPressed;

		#region Debug

		if (printDebug) { Helper.PrintTime("Ship -- OnEnter" + " [" + Enter + "]"); }

		#endregion
	}

	void OnBack(InputValue value)
	{
		Back = value.isPressed;

		#region Debug

		if (printDebug) { Helper.PrintTime("Ship -- OnBack" + " [" + Back + "]"); }

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

		gameObject.GetComponent<PlayerInput>().defaultActionMap = "Gunner";
		gameObject.GetComponent<PlayerInput>().SwitchCurrentActionMap("Gunner");

		Destroy(this);
	}

	#endregion
}