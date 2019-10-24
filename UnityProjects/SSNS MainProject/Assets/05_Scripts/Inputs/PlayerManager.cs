using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
	// Singleton
	public  static PlayerManager Instance { get { return instance; } }
	private static PlayerManager instance;

	// The Players currently active
	public List<Controller> players;

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

	// When a player used unity input system to join let us know
	public void OnPlayerJoined()
	{
		Helper.PrintTime("A New Player Has Joined");
	}

	// When a controller  joins is uses start to add itself to players
	public void Join(Controller newPlayer)
	{
		if (!players.Contains(newPlayer))
		{
			players.Add(newPlayer);
		}
	}
}
