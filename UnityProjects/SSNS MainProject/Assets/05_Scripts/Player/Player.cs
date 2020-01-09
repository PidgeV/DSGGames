using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using SNSSTypes;

public class Player : Controller
{
	// This players role
	public PlayerRole myRole = PlayerRole.None;

	// This players Camera
	[HideInInspector] private Camera myCamera;

	// Start is called before the first frame update
	private void Start()
	{
		// Get the camera from this player AND print a error if it cant be found
		if ((myCamera = GetComponentInChildren<Camera>()) == null) {
			Debug.LogError("You dont appear to have a camera script on this players or its children");
		}

		// Initialize this players role
		InitializeRole(myRole);
	}

	/// <summary>
	/// Initialize this players new role
	/// </summary>
	public void InitializeRole(PlayerRole newRole)
	{
		float screenPercentX = 1.0f;
		float screenPercentY = 1.0f;

		float cameraPos = (int)myRole * 0.5f;

		// Set the players new role
		myRole = newRole;

		// If we dont have a camera do nothing
		if (myCamera == null)
		{
			return;
		}
		else
		{
			// If this player does not have a role
			if (myRole == PlayerRole.None && myCamera)
			{
				screenPercentX = 1.0f;
				screenPercentY = 1.0f;
				cameraPos = 0.0f;
			}

			// If this player is a PILOT
			if (myRole == PlayerRole.Pilot && myCamera)
			{
				screenPercentX = 1.0f;
				screenPercentY = 0.5f;
			}

			// If this player is a GUNNER
			if (myRole == PlayerRole.Gunner && myCamera)
			{
				screenPercentX = 1.0f;
				screenPercentY = 0.5f;
			}

			// Set the new camera rect
			myCamera.rect = new Rect(0, cameraPos, screenPercentX, screenPercentY);
		}	
	}

	// Used to move the player
	public override void OnLeftStick(InputValue input)
	{
		if (myRole == PlayerRole.Pilot)
		{
			// Rotate the player ship
		}

		if (myRole == PlayerRole.Gunner)
		{
			//  Move the player camera
		}
	}

	// Called when this player presses the A button
	public override void OnA(InputValue input)
	{
		Debug.Log("A");

		if (myRole == PlayerRole.Pilot)
		{
			// Boost the ship
		}

		if (myRole == PlayerRole.Gunner)
		{
			// Make the gunner shoot
		}
	}

	// Called when this player presses the B button
	public override void OnB(InputValue input)
	{
		// TODO -- We gota go over what the B button does..

		if (myRole == PlayerRole.Pilot)
		{
		}

		if (myRole == PlayerRole.Gunner)
		{
		}
	}

	// Called when this player presses the X button
	// Used to toggle the map on or off
	public override void OnX(InputValue input)
	{
	}

	// Called when this player presses the Y button
	// Used to ask for a role swap
	public override void OnY(InputValue input)
	{
	}

	// Called when this player uses the DPad button
	public override void OnDPad(InputValue input)
	{
		// NOT -- The DPad does nothing for the pilot

		if (myRole == PlayerRole.Gunner)
		{
			// Change the current weapon
		}
	}

	// Called when this player presses the Left Trigger button
	public override void OnLeftTrigger(InputValue input)
	{
		if (myRole == PlayerRole.Pilot)
		{
			// Boost the ship
		}

		if (myRole == PlayerRole.Gunner)
		{
			// Make the gunner shoot
		}
	}

	// Called when this player presses the Right Trigger button
	public override void OnRightTrigger(InputValue input)
	{
		if (myRole == PlayerRole.Pilot)
		{
			// Boost the ship
		}

		if (myRole == PlayerRole.Gunner)
		{
			// Make the gunner shoot
		}
	}
}
