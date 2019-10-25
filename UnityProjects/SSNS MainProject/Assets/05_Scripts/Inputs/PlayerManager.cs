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

	private Color[] playerColors = new Color[8] { Color.green, Color.red, Color.blue, Color.yellow,
		                                          Color.cyan, Color.gray, Color.magenta, Color.white };

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
		// When a player joins we check if they are already in the list
		// We do nothing if they are
		if (Players.Contains(newPlayer))
		{
			return null;
		}

		int index = 0;

		// We Check the list for an open spot [null] 
		for (index = 0; index < Players.Count; index++)
		{
			// If we have an open slot we add it to the top
			if (Players[index] == null)
			{
				Players[index] = newPlayer;
				break;
			}
		}

		// If we could not find an open slot we add it to the top of the list
		if (index == Players.Count)
		{
			Players.Add(newPlayer);
		}

		Helper.PrintTime("A New Player has joined with the ID - " + index);

		// We return the new player data
		return new PlayerData(index, playerColors[index]);
	}

	/// <summary>
	/// Tell the PlayerManager we are leaving
	/// </summary>
	/// <param name="newPlayer">The player leaving</param>
	public void Disconnect(PlayerData player)
	{
		Players[player.playerID] = null;
	}
}

/// <summary>
/// The Player Data Class holds the data for a Player / Controller
/// </summary>
public class PlayerData
{
	/// <summary> The position in the PlayerManager.Players List  </summary>
	public int playerID = 0;

	/// <summary> The Color that represents this player </summary>
	public Color playerColor;

	// The Constructor
	public PlayerData(int newID, Color color)
	{
		playerID = newID;
		playerColor = color;
	}
}
