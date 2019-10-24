using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The Singleton to the PlayerManager Script
/// </summary>
public class PlayerManager : MonoBehaviour
{
	// The Singleton reference
	public  static PlayerManager Instance { get { return instance; } }
	private static PlayerManager instance;

	/// <summary> The list of Players currently active </summary>
	public List<Controller> Players;

	// Setting up the Singleton
	private void Awake()
	{
		if (instance != null && instance != this)
		{
			Destroy(this.gameObject);
		}
		else
		{
			instance = this;
		}
	}

	/// <summary>
	/// When a player joins this is called from the Controller base class
	/// </summary>
	public PlayerData Join(Controller newPlayer)
	{
		if (Players.Contains(newPlayer))
		{
			return null;
		}

		Players.Add(newPlayer);

		return new PlayerData(Players.Count - 1);
	}
}

/// <summary>
/// The Player Data Class holds the data for a Player / Controller
/// </summary>
public class PlayerData
{
	/// <summary> The position in the PlayerManager.Players List  </summary>
	public int ID = 0;

	// The Constructor
	public PlayerData(int iD)
	{
		ID = iD;
	}
}
