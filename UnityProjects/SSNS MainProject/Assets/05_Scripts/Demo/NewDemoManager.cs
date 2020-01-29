﻿using Complete;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewDemoManager : MonoBehaviour
{
	// The different UI options
	[SerializeField] private Button button;
	[SerializeField] private Button value;
	[SerializeField] private Button vector;

	[Space(5)]
	[SerializeField] private RectTransform playerRect;
	[SerializeField] private RectTransform sceneRect;
	[SerializeField] private RectTransform cameraRect;
	[SerializeField] private RectTransform spawnRect;

	[Space(5)]
	[SerializeField] private GameObject enemy_Charger;
	[SerializeField] private GameObject enemy_Fighter;
	[SerializeField] private GameObject enemy_Cargo;
	[SerializeField] private GameObject enemy_Cruiser;
	[SerializeField] private GameObject enemy_Swarmer;

	[Space(5)]
	[SerializeField] private Camera[] cameras;
	private Camera enabledCam;

	[Space(5)]
	public KeyCode demo_ToggleWindow = KeyCode.Tab;

	// This menus Rect
	private RectTransform menuRect;

	// Menu positions
	Vector3 hiddenPos = Vector3.zero;
	Vector3 initialPos = Vector3.zero;
	Vector3 targetPos = Vector3.zero;

	// Is this menu open ?
	public bool visible = false;

	// Start is called before the first frame update
	void Start()
    {
		if (gameObject.TryGetComponent<RectTransform>(out RectTransform rectTransform))
		{
			initialPos = rectTransform.position;
			hiddenPos = initialPos + new Vector3(rectTransform.rect.width * 1.25f, 0, 0);

			targetPos = hiddenPos;

			menuRect = rectTransform;
			menuRect.position = hiddenPos;

			menuRect.gameObject.SetActive(true);
		}

		Buttons();
	}

    // Update is called once per frame
    void Update()
    {
		if (Vector3.Distance(menuRect.transform.position, targetPos) > 15f)
		{
			// If we're close to our target stop moving
			menuRect.transform.position = Vector3.Lerp(menuRect.transform.position, targetPos, 0.25f);
		}
		else
		{
			// Else we turn off the menu if its hidden
			if (visible == false)
			{
				CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
				canvasGroup.interactable = false;
				canvasGroup.alpha = 0;
			}
		}

		HotKeys();
	}

	/// <summary>
	/// Wait for and respond to hotkey inputs
	/// </summary>
	private void HotKeys()
	{
		if (Input.GetKeyDown(demo_ToggleWindow)) {
		ToggleMenu(!visible);
		}
	}

	/// <summary>
	/// Create Buttons and events without the hassle of dealing with the UI
	/// </summary>
	private void Buttons()
	{
		Button btn_Godmode = CreateButton(button, playerRect, "Toggle Godmode [n/a]");
		btn_Godmode.onClick.AddListener(() => { Debug.LogError("I DONT DO ANYTHING RIGHT NOW REEE [n/a]"); });

		Button btn_TeleportPlayer = CreateButton(vector, playerRect, "Teleport Player [n/a]");
		btn_TeleportPlayer.onClick.AddListener(() => {
			TeleportPlayer(btn_TeleportPlayer.GetComponent<DemoValue>().GetValue);
		});



		Button btn_Spawn_Fighter = CreateButton(button, spawnRect, "Spawn Fighter [n/a]");
		btn_Spawn_Fighter.onClick.AddListener(() => { SpawnEnemy_Fighter();
		});

		Button btn_Spawn_Charger = CreateButton(button, spawnRect, "Spawn Charger [n/a]");
		btn_Spawn_Charger.onClick.AddListener(() => { SpawnEnemy_Charger();
		});

		Button btn_Spawn_Cargo = CreateButton(button, spawnRect, "Spawn Cargo [n/a]");
		btn_Spawn_Cargo.onClick.AddListener(() => { SpawnEnemy_Cargo();
		});

		Button btn_Spawn_Cruiser = CreateButton(button, spawnRect, "Spawn Cruiser [n/a]");
		btn_Spawn_Cruiser.onClick.AddListener(() => { SpawnEnemy_Cruiser();
		});

		Button btn_Spawn_Swarmer = CreateButton(button, spawnRect, "Spawn Swarmer [n/a]");
		btn_Spawn_Swarmer.onClick.AddListener(() => { SpawnEnemy_Swarmer();
		});



		Button btn_Camera01 = CreateButton(button, cameraRect, "Camera 01 [n/a]");
		btn_Camera01.onClick.AddListener(() => {
			UpdateActiveCamera(cameras[0]);
		});

		Button btn_Camera02 = CreateButton(button, cameraRect, "Camera 02 [n/a]");
		btn_Camera02.onClick.AddListener(() => {
			UpdateActiveCamera(cameras[1]);
		});

		Button btn_Camera03 = CreateButton(button, cameraRect, "Camera 03 [n/a]");
		btn_Camera03.onClick.AddListener(() => {
			UpdateActiveCamera(cameras[2]);
		});



		Button btn_ToggleSkybox = CreateButton(button, sceneRect, "Toggle Skybox [n/a]");
		btn_ToggleSkybox.onClick.AddListener(() => {
			SkyboxManager.Instance.LoopSkybox();
		});	

		#region Example Buttons

		// BUTTON
		//Button btn_HelloWorld = CreateButton(button, playerRect, "Click Me :)");
		//btn_HelloWorld.onClick.AddListener(() => { Debug.Log("Hello World!"); });

		// SINGLE VALUE
		//Button btn_GuessANumber = CreateButton(value, playerRect, "Guess a number?");
		//btn_GuessANumber.onClick.AddListener(() => {
		//	DemoValue value = btn_GuessANumber.GetComponent<DemoValue>();
		//	Debug.Log("Was your number " + value.GetX + " ?");
		//});

		// VECTOR
		//Button btn_TeleportPlayer = CreateButton(vector, playerRect, "Teleport Player");
		//btn_TeleportPlayer.onClick.AddListener(() => {
		//	DemoValue value = btn_TeleportPlayer.GetComponent<DemoValue>();
		//	Debug.Log("You want to go to " + value.GetValue + " ?");
		//});
		#endregion
	}

	private Button CreateButton(Button prefab, Transform parent, string name)
	{
		// Make the button
		Button newButton = GameObject.Instantiate(prefab, parent);

		// Name the button
		newButton.GetComponentInChildren<Text>().text = name;

		// return the button
		return newButton;
	}

	#region Helper Methods

	/// <summary>
	/// Move the player to a location
	/// </summary>
	public void TeleportPlayer(Vector3 position)
	{
		GameObject player = GameObject.FindGameObjectWithTag("Player");
		if (player)
		{
			player.transform.position = position;
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

	/// <summary>
	/// Spawn enemies into the scene
	/// </summary>
	/// <param name="enemy">The enemy type</param>
	/// <param name="count">The enemy count</param>
	public void SpawnEnemy(GameObject enemy, Vector3 position, int spawnCount)
	{
		// Loop through each enemy to spawn
		for (int count = 0; count < spawnCount; count++)
		{
			// Instantiate
			GameObject newEnemy = Instantiate(enemy, position, Quaternion.identity);

			// TODO -- We should find a better way to do this
			#region Settings the wapoints 

			// Set waypoints for each enemy
			//if (newEnemy.TryGetComponent<ChaserController>(out ChaserController chaserController)) {
			//	chaserController.waypoints = new GameObject[0];
			//}

			//// Set waypoints for each enemy
			//else if (newEnemy.TryGetComponent<FighterController>(out FighterController fighterController)) {
			//	fighterController.waypoints = new GameObject[0];
			//}
			#endregion
		}
	}

	public void SpawnEnemy_Fighter() {
		if (enemy_Fighter)
		{
			SpawnEnemy(enemy_Fighter, Vector3.zero, Input.GetKey(KeyCode.LeftShift) ? 10 : 1);
		}
		else { Debug.LogError("You do not have the [enemy_Fighter] prefab"); }
	}

	public void SpawnEnemy_Charger() {
		if (enemy_Charger)
		{
			SpawnEnemy(enemy_Charger, Vector3.zero, Input.GetKey(KeyCode.LeftShift) ? 10 : 1);
		}
		else { Debug.LogError("You do not have the [enemy_Charger] prefab"); }
	}

	public void SpawnEnemy_Swarmer()
	{
		if (enemy_Swarmer)
		{
			SpawnEnemy(enemy_Swarmer, Vector3.zero, Input.GetKey(KeyCode.LeftShift) ? 10 : 1);
		}
		else { Debug.LogError("You do not have the [enemy_Swarmer] prefab"); }
	}

	public void SpawnEnemy_Cargo()
	{
		if (enemy_Cargo)
		{
			SpawnEnemy(enemy_Cargo, Vector3.zero, Input.GetKey(KeyCode.LeftShift) ? 10 : 1);
		}
		else { Debug.LogError("You do not have the [enemy_Cargo] prefab"); }
	}

	public void SpawnEnemy_Cruiser()
	{
		if (enemy_Cruiser)
		{
			SpawnEnemy(enemy_Cruiser, Vector3.zero, Input.GetKey(KeyCode.LeftShift) ? 10 : 1);
		}
		else { Debug.LogError("You do not have the [enemy_Cruiser] prefab"); }
	}

	/// <summary>
	/// Toggle the visability of the Demo window
	/// </summary>
	public void ToggleMenu(bool state)
	{
		CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
		canvasGroup.interactable = transform;
		canvasGroup.alpha = 1;

		visible = state;
		targetPos = (visible) ? initialPos : hiddenPos;
	}

	#endregion
}
