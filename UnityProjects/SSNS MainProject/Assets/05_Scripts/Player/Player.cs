using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using SNSSTypes;

public enum ControlType
{
	CONTROLLER,
	KEYBOARD_AND_MOUSE
}

public class Player : Controller
{
	// This players role (Pilot or Gunner)
	public PlayerRole myRole = PlayerRole.None;

	// The current gamestate
	// This is used to change the playing action map to a menu action map
	public GameState currentState = GameState.BATTLE;

	// The Control Type (Controller or Keyboard)
	public ControlType controlType = ControlType.CONTROLLER;

	public PlayerInput PlayerInput;
	public ShipController Controller;

	#region Unity Events

	private void Awake()
	{
		PlayerInput = GetComponent<PlayerInput>();
		// When a player starts the game this grabs an open ship
		foreach (GameObject ship in GameObject.FindGameObjectsWithTag("Player"))
		{
			if (ship.TryGetComponent(out ShipController shipController))
			{
				Controller = shipController;
				Controller.JoinShip(this);
				break;
			}
		}
	}

	private void Start()
	{
		PlayerConnectionManager.Instance.Join(this);
	}

	private void Update()
	{
		// Keep our GameState up to date with the GameManager
		if (GameManager.Instance && currentState != GameManager.Instance.GameState)
		{
			currentState = GameManager.Instance.GameState;
			SetPlayerActionMap(currentState);
		}

		#region Not Working
		//// ONLY.. If were using a mouse and keyboard
		//if (controlType == ControlType.KEYBOARD_AND_MOUSE)
		//{
		//	Vector2 screenCenter = new Vector2(Screen.width, Screen.height - (Screen.height / 2)) / 2f;
		//	Vector2 mousePosition = Input.mousePosition;

		//	if (myRole == PlayerRole.Gunner)
		//	{
		//		if (mousePosition.y < (Screen.height / 2))
		//		{
		//			Vector2 direction = mousePosition - screenCenter;

		//			direction.x = direction.x / Screen.width;
		//			direction.y = direction.y / Screen.height;

		//			controller.AimGun(direction * 2);
		//		}
		//		else
		//		{
		//			controller.AimGun(new Vector2(0, 1));
		//		}
		//	}

		//}
		#endregion
	}
	#endregion

	//
	public void SetPlayerActionMap(GameState currentState)
	{
		if (currentState == GameState.NODE_SELECTION)
		{
			Debug.Log("Switch Current ActionMap (NodeMap)");
			PlayerInput.SwitchCurrentActionMap("NodeMap");
		}
		else if (currentState == GameState.BATTLE)
		{
			Debug.Log("Switch Current ActionMap (Ship)");
			PlayerInput.SwitchCurrentActionMap("Ship");
		}
	}

	#region [Action Map] NodeMap
	public void OnNavigate(InputValue input)
	{
		if (myRole == PlayerRole.Pilot)
		{
			if (input.Get<Vector2>().x >= 0.05f)
			{
				NodeManager.Instance.SelectNodeChoice(1);
			}
			else if (input.Get<Vector2>().x <= -0.05f)
			{
				NodeManager.Instance.SelectNodeChoice(-1);
			}
		}
	}

	#endregion

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
			Controller.SteerShip(input.Get<Vector2>());
		}

		if (myRole == PlayerRole.Gunner)
		{
			// Move the player camera
			Controller.AimGun(input.Get<Vector2>());
		}
	}
	public override void OnRightStick(InputValue input)
	{
		if (myRole == PlayerRole.Pilot)
		{
			// Rotate the player ship
			Controller.StrafeShip(input.Get<Vector2>());
		}

		if (myRole == PlayerRole.Gunner)
		{
			// Move the player camera
			Controller.AimGun(input.Get<Vector2>());
		}
	}

	// Called when this player uses the DPad
	public override void OnDPad(InputValue input)
	{
		// NOTE -- The DPad does nothing for the pilot

		if (myRole == PlayerRole.Gunner)
		{
			// Change the current weapon
			Controller.SwapWeapon(input.Get<Vector2>());
		}
	}

	public override void OnA(InputValue input)
	{
		if (myRole == PlayerRole.Pilot)
		{
			// Boost the ship
			Controller.Boost(input.isPressed);
		}

		if (myRole == PlayerRole.Gunner)
		{
			// Make the gunner shoot
			Controller.Shoot(myRole, input.isPressed);
		}
	}
	public override void OnB(InputValue input)
	{
		// TODO -- We gota go over what the B button does..

		if (myRole == PlayerRole.Pilot)
		{
			// Boost the ship
			Controller.Boost(input.isPressed);
		}

		if (myRole == PlayerRole.Gunner)
		{
			// Make the gunner shoot
			Controller.Shoot(myRole, input.isPressed);
		}
	}
	public override void OnX(InputValue input)
	{
		Controller.ToggleMap(input.isPressed);
	}
	public override void OnY(InputValue input)
	{
		if (myRole != PlayerRole.None) Controller.TriggerRoleSwap(input.isPressed);
	}

	public override void OnLeftTrigger(InputValue input)
	{
		if (myRole == PlayerRole.Pilot)
		{
			// Boost the ship
			Controller.RotateShip(input.Get<float>());
		}

		if (myRole == PlayerRole.Gunner)
		{
			// Make the gunner shoot
			Controller.Shoot(myRole, input.isPressed);
		}
	}
	public override void OnRightTrigger(InputValue input)
	{
		if (myRole == PlayerRole.Pilot)
		{
			// Boost the ship
			Controller.RotateShip(-input.Get<float>());
		}

		if (myRole == PlayerRole.Gunner)
		{
			// Make the gunner shoot
			Controller.Shoot(myRole, input.isPressed);
		}
	}

	public override void OnRightBumper(InputValue input)
	{
		Controller.Shoot(myRole, input.isPressed);
	}
	public override void OnLeftBumper(InputValue input)
	{
		Controller.LockOn(input.isPressed);
	}
	#endregion

	#region [Action Map] Menu Navigation

	public void OnPause(InputValue input)
	{
		if (input.isPressed == false)
		{
			GameManager.Instance.PauseGame();
		}
	}

	public void OnNavigate()
	{
		// Unity automatically does this
	}

	public void OnSelect()
	{
		// Unity automatically does this
	}

	public void OnBack()
	{
		CinematicController.Instance.Return();
	}

	#endregion
}
