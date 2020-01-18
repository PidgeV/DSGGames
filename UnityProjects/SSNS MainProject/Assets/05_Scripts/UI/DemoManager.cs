﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering.HDPipeline;
using Complete;

/// <summary>
/// Keybindings:
/// 
/// Number keys are to switch between cameras
/// M to spawn charger enemy
/// N to spawn fighter enemy
/// T randomizes skybox
/// </summary>
public class DemoManager : MonoBehaviour
{
	[SerializeField] private DemoManagerMenu DemoMenu;

	// Move the scene camera to..
	#region Set and Move Cameras

	[Header("Cameras")]
	public Camera[] cameras;
	Camera enabledCam;

	// NOTE -- Keypad is using the NumPad not the number keys

	// Camera inputs
	public KeyCode camera_01 = KeyCode.Keypad1; // The Player
	public KeyCode camera_02 = KeyCode.Keypad2; // The Dreadnova
	public KeyCode camera_03 = KeyCode.Keypad3; // Fighter Enemy
	public KeyCode camera_04 = KeyCode.Keypad4; // Charger Enemy
	public KeyCode camera_05 = KeyCode.Keypad5; // The Environment

	#endregion

	// Set player options
	#region Player Options

	[Header("Player Options")]
	// Player inputs
	public KeyCode player_Godmode = KeyCode.G;
	public KeyCode player_Teleport = KeyCode.T;

	private GameObject playerObj;

	#endregion

	// Spawn Enemies
	#region Spawn Enemies

	[Header("Enemy Spawning")]
	public GameObject chargerPrefab;
	public GameObject fighterPrefab;
	[SerializeField] GameObject[] waypoints;

	GameObject spawnedEnemy;

	// Spawn inputs
	public KeyCode spawn_Charger = KeyCode.M;
	public KeyCode spawn_Fighter = KeyCode.N;

	#endregion

	// Change the skybox
	#region Change Skybox

	[Header("Skybox")]
	[SerializeField] Cubemap[] skyboxes; // List of skyboxes in game

	HDRISky skybox;
	Volume profile;

	int skyboxIndex = 1;

	// Skybox inputs
	public KeyCode skyBox_Change = KeyCode.U;

	#endregion

	// Demo Manager
	#region Demo Manager

	[Header("Demo Options")]
	public KeyCode demo_ToggleWindow = KeyCode.Tab;
	public KeyCode demo_PauseGame = KeyCode.P;

	#endregion

	// TODO -- Play Animations
	#region Play Animations

	// Cinematics
	// Open -> MainMenu
	// MainMenu -> Options
	// MainMenu -> GameSetup
	// GameSetup -> NewGame / LoadGame

	// (Player) Entering a new node
	// (Player) Wapring to the next node

	// (Player) Game Over

	// (Dreadnove) Warp into scene
	// (Dreadnove) Part Destruction
	// (Dreadnove) Shields up
	// (Dreadnove) Shields down

	// ect

	#endregion

	// TODO -- Scene Manager
	#region Scene Manager

	// Kill all the enemies in the scene
	// Reset the scene
	// Load a new scene

	#endregion

	// Start is called before the first frame update
	void Start()
	{
		DemoMenu.gameObject.SetActive(true);

		// Find the player
		playerObj = GameObject.FindGameObjectWithTag("Player");

		if (playerObj == null)
		{
			Debug.Log("We could not find a player object!");
		}

		if (cameras.Length > 0)
		{
			// Loop through each camera and disable them
			foreach (Camera camera in cameras)
			{
				camera.enabled = false;
			}

			enabledCam = cameras[0];
			enabledCam.enabled = true;
		}

		// Try to get a reference to the Skybox
		if (profile = GameObject.FindObjectOfType<Volume>())
		{
			profile.profile.TryGet(out skybox);
		}
		else
		{
			Debug.LogError("We could not find the Volume profile in your scene");
		}
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyDown(skyBox_Change) && skybox)
		{
			// Change the skybox to the next in the list
			ToggleSkybox();
		}

		if (Input.GetKeyDown(player_Godmode) && playerObj)
		{
			// Toggle on god mode for the player
			if (playerObj.TryGetComponent<HealthAndShields>(out HealthAndShields healthAndShields))
			{
				healthAndShields.godmode = !healthAndShields.godmode;
			}
		}

		if (Input.GetKeyDown(player_Teleport) && playerObj)
		{
			// TODO -- TP the player to a location
			// Using the hotkey it could tp the player to 0 0 0 
			TeleportPlayer();
		}

		if (Input.GetKeyDown(demo_PauseGame))
		{
			// TODO -- Pause the game
		}

		if (Input.GetKeyDown(demo_ToggleWindow) && DemoMenu)
		{
			DemoMenu.ToggleMenu(!DemoMenu.visible);
		}

		// Read and Act on inputs for the camera
		ChangeCamera();

		// Spawn enemies into the scene for testing
		SpawnEnemy();
	}

	/// <summary>
	/// Read input for Spawning enemies
	/// </summary>
	void SpawnEnemy()
	{
		// NOTE -- If shift is held down it will spawn 10 enemies instead of 1

		// Spawn Chargers
		if (Input.GetKeyDown(spawn_Charger) && chargerPrefab)
		{
			SpawnEnemy(chargerPrefab, Vector3.zero, Input.GetKey(KeyCode.LeftShift) ? 10 : 1);
		}

		// Spawn Fighters
		if (Input.GetKeyDown(spawn_Fighter) && fighterPrefab)
		{
			SpawnEnemy(fighterPrefab, Vector3.zero, Input.GetKey(KeyCode.LeftShift) ? 10 : 1);
		}
	}

	/// <summary>
	/// Change the skybox
	/// </summary>
	public void ToggleSkybox()
	{
		skybox.hdriSky.Override(skyboxes[(skyboxIndex++ % skyboxes.Length)]);
	}

	/// <summary>
	/// Move the player
	/// </summary>
	public void TeleportPlayer()
	{
		foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
		{
			player.transform.position = Vector3.zero;
		}
	}

	/// <summary>
	/// Spawn enemies into the scene
	/// </summary>
	/// <param name="enemy">The enemy type</param>
	/// <param name="count">The enemy count</param>
	public void SpawnEnemy(GameObject enemy, Vector3 position, int spawnCount = 1)
	{
		// Loop through each enemy to spawn
		for (int count = 0; count < spawnCount; count++)
		{
			// Instantiate
			GameObject newEnemy = Instantiate(enemy, position, Quaternion.identity);

			// TODO -- We should find a better way to do this
			#region Settings the wapoints 

			// Set waypoints for each enemy
			if (newEnemy.TryGetComponent<ChaserController>(out ChaserController chaserController))
			{
				chaserController.waypoints = waypoints;
			}

			// Set waypoints for each enemy
			if (newEnemy.TryGetComponent<FighterController>(out FighterController fighterController))
			{
				fighterController.waypoints = waypoints;
			}

			#endregion
		}
	}

	/// <summary>
	/// Handle the changing of cameras in the scene
	/// </summary>
	void ChangeCamera()
	{
		// Camera 1
		if (Input.GetKeyDown(camera_01) && cameras.Length >= 1)
		{
			UpdateActiveCamera(cameras[0]);
		}

		// Camera 2
		if (Input.GetKeyDown(camera_02) && cameras.Length >= 2)
		{
			UpdateActiveCamera(cameras[1]);
		}

		// Camera 3
		if (Input.GetKeyDown(camera_03) && cameras.Length >= 3)
		{
			UpdateActiveCamera(cameras[2]);
		}

		// [Unused] Camera 4
		if (Input.GetKeyDown(camera_04) && cameras.Length >= 4)
		{
			UpdateActiveCamera(cameras[3]);
		}

		// [Unused] Camera 5
		if (Input.GetKeyDown(camera_05) && cameras.Length >= 5)
		{
			UpdateActiveCamera(cameras[4]);
		}
	}

	/// <summary>
	/// Update the currently active camera and turn off the old camera
	/// </summary>
	public void UpdateActiveCamera(Camera newCamera)
	{
		// Disable the old camera
		enabledCam.enabled = false;

		// Enable the new camera
		enabledCam = newCamera;
		enabledCam.enabled = true;
	}
}
