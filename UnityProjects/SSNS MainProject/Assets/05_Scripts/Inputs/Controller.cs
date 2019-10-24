using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The base class that all input scripts inherit from
/// </summary>
public class Controller : MonoBehaviour
{
	/// <summary> The information about this player </summary>
	public PlayerData PlayerData;

	/// <summary>
	/// When a player / controller joins the game this is called from a derived class
	/// The JoinGame() function initializes this player's data
	/// </summary>
	public void JoinGame()
	{
		PlayerData = PlayerManager.Instance.Join(this);
	}
}
