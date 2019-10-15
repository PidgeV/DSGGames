using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// The NoJobInputs is the default class for inputs 
// When a new controller connects it is of type NoJobInputs

// Click right on the d-pad to become a Gunner
// Click left on the d-pad to become a Piolet 

public class NoJobInputs : MonoBehaviour
{
	void OnMove(InputValue value)
	{
	}

	void OnEnter()
	{
	}

	void OnBack()
	{
	}

	// Removes the NoJobInputs and gives the player the PlayerInput 
	void OnConvertToPiolet()
	{
		Debug.Log("Converting.. [ Pilot Inputs Activated ]");

		gameObject.AddComponent<ShipInput>();
		gameObject.GetComponent<PlayerInput>().SwitchCurrentActionMap("Ship");

		Destroy(this);
	}

	// Removes the NoJobInputs and gives the player the GunnerInputs 
	void OnConvertToGunner()
	{
		Debug.Log("Converting.. [ Gunner Inputs Activated ]");

		gameObject.AddComponent<GunnerInput>();
		gameObject.GetComponent<PlayerInput>().SwitchCurrentActionMap("Gunner");

		Destroy(this);
	}
}
