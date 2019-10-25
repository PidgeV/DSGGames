using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The base class that all input scripts inherit from.
/// </summary>
public class Controller : MonoBehaviour
{
	/// <summary> The information about this player. </summary>
	public PlayerData PlayerData;

	#region Menu Navigation

	/// <summary> If the player is controlling a Menu. </summary>
	public bool MenuMode = false;

	/// <summary> The sensitivity of an input to register for a menu change [ 0 - 1]. </summary>
	public float Sensitivity = 0.2f;

	// The Time since our last menu input
	protected float menuCounter = 0f;

	// The Minimum  time between menu inputs
	protected float menuChangeTime = 0.15f;

	#endregion

	// DEBUG
	public bool printDebug = false;

	/// <summary>
	/// When a player / controller joins the game this is called from a derived class.
	/// The JoinGame() function initializes this player's data.
	/// </summary>
	public void JoinGame()
	{
		PlayerData = PlayerManager.Instance.Join(this);
	}

	/// <summary>
	/// Send an input direction to the Menu
	/// </summary>
	public void SendMenuInput(Vector2 input)
	{
		// LEFT
		if (input.x < -Sensitivity)
		{
			UIManager.Instance.TransitionElement(Helper.eMenuDirection.LEFT);
		}

		// RIGHT
		if (input.x > Sensitivity)
		{
			UIManager.Instance.TransitionElement(Helper.eMenuDirection.RIGHT);
		}

		// UP
		if (input.y > Sensitivity)
		{
			UIManager.Instance.TransitionElement(Helper.eMenuDirection.UP);
		}

		// DOWN
		if (input.y < -Sensitivity)
		{
			UIManager.Instance.TransitionElement(Helper.eMenuDirection.DOWN);
		}
	}
}
