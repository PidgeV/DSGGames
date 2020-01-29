using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using SNSSTypes;

public class Player : Controller
{
	// This players role
	public PlayerRole myRole = PlayerRole.None;
	private GameState currentState = GameState.BATTLE;

	PlayerInput playerInput;
	testShipController controller;

	// Start is called before the first frame update
	private void Awake()
	{
		playerInput = GetComponent<PlayerInput>();
		foreach (GameObject ship in GameObject.FindGameObjectsWithTag("Player"))
		{
			if (ship.TryGetComponent<testShipController>(out testShipController shipController))
			{
				controller = shipController;
				controller.JoinShip(this);
				break;
			}
		}
	}

	//
	private void Update()
	{
		if (GameManager.Instance && currentState != GameManager.Instance.GameState)
		{
			currentState = GameManager.Instance.GameState;
			SetPlayerActions(currentState);
		}
	}
	 
	//
	public void SetPlayerActions(GameState currentState)
	{
		if (currentState == GameState.NODE_SELECTION)
		{
			Debug.Log("Switch Current ActionMap (NodeMap)");
			playerInput.SwitchCurrentActionMap("NodeMap");
		}
		else if (currentState == GameState.BATTLE)
		{
			Debug.Log("Switch Current ActionMap (Ship)");
			playerInput.SwitchCurrentActionMap("Ship");
		}
	}

	#region [Action Map] Ship Controller
	/// <summary> Spawn the players role </summary>
	public void SwapRole()
	{
		if (myRole == PlayerRole.None)
		{
			Debug.Log("You cannot swap rolls if you do not have one!");
		}
		else
		{
			if (myRole == PlayerRole.Pilot)
			{
				myRole = PlayerRole.Gunner;
				return;
			}

			if (myRole == PlayerRole.Gunner)
			{
				myRole = PlayerRole.Pilot;
				return;
			}
		}
	}

	// Used to move the player
	public override void OnLeftStick(InputValue input)
	{
		if (myRole == PlayerRole.Pilot)
		{
			// Rotate the player ship
			controller.SteerShip(input.Get<Vector2>());
		}

		if (myRole == PlayerRole.Gunner)
		{
			// Move the player camera
			controller.AimGun(input.Get<Vector2>());
		}
	}

	// Used to move the player
	public override void OnRightStick(InputValue input)
	{
		if (myRole == PlayerRole.Pilot)
		{
			// Rotate the player ship
			controller.StrafeShip(input.Get<Vector2>());
		}

		if (myRole == PlayerRole.Gunner)
		{
			// Move the player camera
			controller.AimGun(input.Get<Vector2>());
		}
	}

	// Called when this player uses the DPad
	public override void OnDPad(InputValue input)
	{
		// NOTE -- The DPad does nothing for the pilot

		if (myRole == PlayerRole.Gunner)
		{
			// Change the current weapon
			controller.SwapWeapon(input.Get<Vector2>());
		}
	}

	// Called when this player presses the A button
	public override void OnA(InputValue input)
	{
		if (myRole == PlayerRole.Pilot)
		{
			// Boost the ship
			controller.Boost(input.isPressed);
		}

		if (myRole == PlayerRole.Gunner)
		{
			// Make the gunner shoot
			controller.Shoot(input.isPressed);
		}
	}

	// Called when this player presses the B button
	public override void OnB(InputValue input)
	{
		// TODO -- We gota go over what the B button does..

		if (myRole == PlayerRole.Pilot)
		{
			// Boost the ship
			controller.Boost(input.isPressed);
		}

		if (myRole == PlayerRole.Gunner)
		{
			// Make the gunner shoot
			controller.Shoot(input.isPressed);
		}
	}

	// Called when this player presses the X button
	// Used to toggle the map on or off
	public override void OnX(InputValue input)
	{
		controller.ToggleMap(input.isPressed);
	}

	// Called when this player presses the Y button
	// Used to ask for a role swap
	public override void OnY(InputValue input)
	{
		controller.TriggerRoleSwap(input.isPressed);
	}

	// Called when this player presses the Left Trigger
	public override void OnLeftTrigger(InputValue input)
	{
		if (myRole == PlayerRole.Pilot)
		{
			// Boost the ship
			controller.RotateShip(input.Get<float>());
		}

		if (myRole == PlayerRole.Gunner)
		{
			// Make the gunner shoot
			controller.Shoot(input.isPressed);
		}
	}

	// Called when this player presses the Right Trigger
	public override void OnRightTrigger(InputValue input)
	{
		if (myRole == PlayerRole.Pilot)
		{
			// Boost the ship
			controller.RotateShip(-input.Get<float>());
		}

		if (myRole == PlayerRole.Gunner)
		{
			// Make the gunner shoot
			controller.Shoot(input.isPressed);
		}
	}
	#endregion

	#region [Action Map] NodeMap
	public void OnNavigate(InputValue input)
	{
		float value = 0;
		if (myRole == PlayerRole.Pilot)
		{
			value = input.Get<Vector2>().x;
		}
		else if (myRole == PlayerRole.Gunner)
		{
			value = -input.Get<Vector2>().y;
		}

		if (value >= 0.05f)
		{
			NodeManager.Instance.SelectRightNode(myRole);
		}
		else if (value <= -0.05f)
		{
			NodeManager.Instance.SelectLeftNode(myRole);
		}
	}

	public void OnConfirm(InputValue input)
	{
		Debug.Log("OnConfirm");
		NodeManager.Instance.PlayerConfirm(myRole, true);
	}

	public void OnDecline(InputValue input)
	{
		NodeManager.Instance.PlayerConfirm(myRole, false);
	}
	#endregion
}
