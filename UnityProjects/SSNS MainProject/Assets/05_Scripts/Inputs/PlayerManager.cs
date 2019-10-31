using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// The Singleton to the PlayerManager Script
/// </summary>
public class PlayerManager : MonoBehaviour
{
	// The Singleton reference
	public static PlayerManager Instance { get { return instance; } }
	private static PlayerManager instance;

	private Color[] playerColors = new Color[0];

	/// <summary> The list of Players currently active </summary>
	public List<Controller> Players;

	// List of connections
	private List<GameObject> connections = new List<GameObject>();

	// The Layout Group
	public HorizontalLayoutGroup layoutGroup;

	// The connections prefab
	public GameObject connectionsPrefab;

	public TeamController teamControllerPrefab;

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
			DontDestroyOnLoad(gameObject);
		}

		playerColors = new Color[8]
		{
			Color.green,
			Color.red,
			Color.blue,
			Color.yellow,
			Color.cyan,
			Color.gray,
			Color.magenta,
			Color.white
		};
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

	/// <summary>
	/// Upedate the player UI Menu
	/// </summary>
	public void UpedatePlayerUI()
	{
		foreach (GameObject gameObject in connections)
		{
			Destroy(gameObject);
		}
		connections.Clear();

		int index = 0;
		foreach (Controller player in Players)
		{
			if (player)
			{
				connections.Add(GameObject.Instantiate(connectionsPrefab, layoutGroup.transform));
				connections[connections.Count - 1].GetComponent<PlayerConnection>().Initialize(Players[index].PlayerData);
			}

			index++;
		}
	}

	// Resets the connections
	public void ClearPlayers()
	{
		foreach (GameObject gameObject in connections)
		{
			Destroy(gameObject);
		}
		connections.Clear();

		foreach (Controller player in Players)
		{
			if (player)
			{
				Destroy(player.gameObject);
			}
		}
	}

	public void SetUpTeams()
	{
		TeamController teamController = null;

		GameObject ship;

		int cameraIndex = -1;

		bool IsPilot = true;
		for (int index = 0; index < Players.Count; index++)
		{
			if (IsPilot)
			{
				cameraIndex++;
				teamController = Instantiate(teamControllerPrefab);
				DontDestroyOnLoad(teamController.gameObject);

				if (Display.displays.Length > cameraIndex && cameraIndex != 0)
					Display.displays[cameraIndex].Activate();

				teamController.SetCameras(cameraIndex);
			}

			if (IsPilot)
			{
				Players[index].gameObject.GetComponent<IdleInputs>().OnConvertToPiolet();
				teamController.AssignController(Players[index].GetComponent<PilotController>());
			}
			else
			{
				Players[index].gameObject.GetComponent<IdleInputs>().OnConvertToGunner();
				teamController.AssignController(Players[index].GetComponent<TurretController>());
			}

			IsPilot = !IsPilot;
		}

		SceneManager.LoadScene(1);
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

	public string GetName { get { return "Player " + (playerID + 1); } }

	// The Constructor
	public PlayerData(int newID, Color color)
	{
		playerID = newID;
		playerColor = color;
	}
}
