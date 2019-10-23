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

	#region Menu Navigation

	// The menu the is currently selected 
	public MenuElement selectedMenu;

	float menuCounter = 0f;
	float menuChangeTime = 0.15f;

	#endregion

	// DEBUG
	public bool printDebug = false;

	// Start is called before the first frame update
	private void Start()
	{
		MenuElement initialMenu = GameObject.FindGameObjectWithTag("FirstMenuElement").GetComponent<MenuElement>();

		selectedMenu = initialMenu;
	}

	private void Update()
	{
		// Increment the timer
		menuCounter += Time.deltaTime;
		
		if (menuCounter > menuChangeTime)
		{
			// Reset the counter
			menuCounter = 0f;

			float sensitivity = 0.5f;

			if (move.x < -sensitivity)
			{
				// LEFT
				selectedMenu.TransitionElement(this, Helper.eMenuDirection.LEFT);
			}
			else if (move.x > sensitivity)
			{
				// RIGHT
				selectedMenu.TransitionElement(this, Helper.eMenuDirection.RIGHT);
			}
			else if (move.y > sensitivity)
			{
				// UP
				selectedMenu.TransitionElement(this, Helper.eMenuDirection.UP);
			}
			else if (move.y < -sensitivity)
			{
				// DOWN
				selectedMenu.TransitionElement(this, Helper.eMenuDirection.DOWN);
			}
		}
	}

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