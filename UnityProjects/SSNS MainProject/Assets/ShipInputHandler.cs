using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SNSSTypes;

public class ShipInputHandler : MonoBehaviour
{
	private ShipController _shipController;

	private void Awake()
	{
		_shipController = GetComponent<ShipController>();
	}

	/// <summary> Add a player to the ship </summary>
	public void JoinShip(Player newPlayer)
	{
		// Check if we are the first player to join
		if (_shipController.player1)
		{
			// If we have a pilot

			// Make the new player a gunner
			if (_shipController.player1.myRole == PlayerRole.Pilot)
			{
				_shipController.player2 = newPlayer;
				_shipController.player2.myRole = PlayerRole.Gunner;
			}
			else
			{
				_shipController.player2 = newPlayer;
				_shipController.player2.myRole = PlayerRole.Pilot;
			}
		}
		else
		{
			// If we dont have a pilot

			// Make the new player a pilot
			_shipController.player1 = newPlayer;
			_shipController.player1.myRole = PlayerRole.Pilot;
		}

		// Increment the number of players
		_shipController.NumberOfPlayers++;
	}

	/// <summary> Make the ship shoot </summary>
	public void ShootGun(bool pressed)
	{
		_shipController.ShootGun(pressed);
	}

	/// <summary> Make the ship shoot </summary>
	public void ShootShip( bool pressed)
	{
		_shipController.ShootShip(pressed);
	}

	/// <summary> Swap the ships weapon  </summary>
	public void SwapWeapon(Vector2 Input)
	{
		_shipController.SwapWeapon(Input);
	}

	/// <summary> Steer the ship </summary>
	public void SteerShip(Vector2 velocity)
	{
		// Check if we are steering
		if (velocity == Vector2.zero)
		{
			_shipController.RotateDirection = Vector2.zero;
			_shipController.Rotating = false;
		}
		else
		{
			// Check if we want inverted controls
			if (_shipController.invertedControls == true)
			{
				_shipController.RotateDirection = new Vector2(velocity.y, velocity.x) * _shipController.TurnMultiplayer;
			}
			else
			{
				_shipController.RotateDirection = new Vector2(-velocity.y, velocity.x) * _shipController.TurnMultiplayer;
			}

			_shipController.Rotating = true;
		}
	}

	/// <summary> Move the gunners camera </summary>
	public void AimGun(Vector2 velocity)
	{
		_shipController.GunVelocity = new Vector2(velocity.x, velocity.y) * _shipController.GunRotationSpeed;
	}

	/// <summary> Move the Ship up, down, left or right </summary>
	public void StrafeShip(Vector2 velocity)
	{
		if (velocity.sqrMagnitude >= 1)
		{
			_shipController.StrafeDirection = velocity;
			_shipController.Strafing = true;
		}
		else
		{
			_shipController.Strafing = false;
		}
	}

	/// <summary> Rotate the ship on the Z axis </summary>
	public void RotateShip(float direction)
	{
		_shipController.RollInput = direction;
	}

	/// <summary> Change the roles of the players </summary>
	public void SwapRoles()
	{
		if (_shipController.player1) {
			_shipController.player1.SwapRole();
		}

		if (_shipController.player2) {
			_shipController.player2.SwapRole();
		}

		_shipController.GunVelocity = Vector3.zero;
	}

	/// <summary> Trigger a role swap request </summary>
	public void TriggerRoleSwap(bool pressed)
	{
		// If we only have one player..
		// Just let them swap on command
		if (_shipController.NumberOfPlayers == 1 && pressed == true)
		{
			_shipController.SwapRoles();
			return;
		}

		// If were currently looking for a role swap
		if (_shipController.RoleSwap == true && pressed == true)
		{
			// This means that BOTH players are holding down the role swap button
			_shipController.SwapRoles();
		}
		else
		{
			// Else we want to set looking for roleSwap to whatever the last input was

			// This could be a button press -> start a roleSwap
			// This could be a button release -> cancel the roleSwap

			_shipController.RoleSwap = pressed;
		}
	}

	/// <summary> Toggle the boosting member </summary>
	public void Boost(bool pressed)
	{
		// If our boost gauge is more than 5% full we allow the player to boost
		// This is to avoid stuttering  on a 0% gauge
		if (_shipController.BoostGauge > _shipController.myStats.maxBoostGauge * 0.1f)
		{
			if (pressed)
			{
				_shipController.warpEffect1.Play();
				_shipController.warpEffect2.Play();
			}
			else
			{
				_shipController.warpEffect1.Stop();
				_shipController.warpEffect2.Stop();
			}

			_shipController.Boosting = pressed;
		}
		else
		{
			_shipController.warpEffect1.Stop();
			_shipController.warpEffect2.Stop();

			_shipController.Boosting = false;
		}
	}

	/// <summary> Toggle the ships map </summary>
	public void ToggleMap(bool pressed)
	{
	}

	/// <summary> Toggle the lockOn member </summary>
	public void LockOn(bool pressed)
	{
	}
}
