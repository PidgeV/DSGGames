using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// The NoJobInputs is the default class for inputs 
// When a new controller connects it is of type NoJobInputs

// [WILL BE REMOVED]
// Click right on the d-pad to become a Gunner
// Click left on the d-pad to become a Piolet 

public class NoJobInputs : Controller
{
	// The last input -- This is the same as Input.GetAxis();
	public Vector2 move = new Vector2();

	#region Menu Navigation

	// The sensitivity of an input to register for a menu change [ 0 - 1]
	float sensitivity = 0.2f;

	// The Time since our last menu input
	float menuCounter = 0f;

	// The Minimum  time between menu inputs
	float menuChangeTime = 0.15f;

	#endregion

	// DEBUG
	public bool printDebug = false;

	// Start is called before the first frame update
	private void Start()
	{
		// Join the game
		JoinGame();
	}

	private void Update()
	{
		HandleMenuInputs();
	}

	// The Standard methods like HandleMenuInputs()
	#region Class Methods

	private void HandleMenuInputs()
	{
		// Increment the timer
		menuCounter += Time.deltaTime;

		if (menuCounter > menuChangeTime)
		{
			// Reset the counter
			menuCounter = 0f;

			// LEFT
			if (move.x < -sensitivity)
			{
				UIManager.Instance.TransitionElement(Helper.eMenuDirection.LEFT);
			}

			// RIGHT
			if (move.x > sensitivity)
			{
				UIManager.Instance.TransitionElement(Helper.eMenuDirection.RIGHT);
			}

			// UP
			if (move.y > sensitivity)
			{
				UIManager.Instance.TransitionElement(Helper.eMenuDirection.UP);
			}

			// DOWN
			if (move.y < -sensitivity)
			{
				UIManager.Instance.TransitionElement(Helper.eMenuDirection.DOWN);
			}
		}
	}

	#endregion

	// IF YOU WANT CODE TO ACTIVATE ON A BUTTON CLICK USE THESE

	// NOTE -- These are the input functions called AUTOMATICALLY by the Player Input Script 
	//		   [Part of unitys new input system]

	#region Input Functions

	void OnMove(InputValue value)
	{
		move = value.Get<Vector2>();

		#region Debug

		if (printDebug) { Helper.PrintTime("Controller -- OnMove" + "[ " + move + " ]"); }

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