using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SNSSTypes;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles switching from states
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public bool debug;
    public bool paused = false;

    [SerializeField] private GameState gameState = GameState.WARPING;

    [SerializeField] private int seed;

    private System.Random random;

    private ShipController shipController;

    private const int MAX_RESPAWN_TIME = 5;

    private float respawnTime;

	/// <summary>
	/// Pause the game and open the pause menu
	/// This is called from the players OnPause
	/// </summary>
	public void PauseGame()
	{
		Player[] players = FindObjectsOfType<Player>();

		foreach (Player player in players)
		{
			string newActions = !paused ? "MenuNavigation" : "Ship";
			player.PlayerInput.SwitchCurrentActionMap(newActions);
		}

		paused = !paused;

		GameObject pauseMenu = GameObject.Find("PauseMenu");
		if (pauseMenu && pauseMenu.TryGetComponent<Animator>(out Animator animator))
		{
			bool open = animator.GetBool("Open");
			animator.SetBool("Open", !open);
		}
	}

    /// <summary>
    /// Switch to provided state
    /// </summary>
    /// <param name="gameState">The game state to switch to</param>
    public void SwitchState(GameState gameState)
    {
        this.gameState = gameState;

        UpdateState();
    }

    /// <summary>
    /// Transition to new state
    /// </summary>
    private void UpdateState()
    {
        if (!shipController)
            shipController = GameObject.FindGameObjectWithTag("Player").GetComponent<ShipController>();

        HealthAndShields healthAndShields = shipController.GetComponent<HealthAndShields>();

        switch (gameState)
        {
            case GameState.BATTLE:
                MusicManager.instance.RandomTrack(MusicTrackType.COMBAT);
                healthAndShields.Invincible = false;
                shipController.Warping = false;
                WarpEffectBehaviour.instance.EndWarp();
                break;
            case GameState.BATTLE_END:
                MusicManager.instance.RandomTrack(MusicTrackType.NON_COMBAT);
				healthAndShields.ResetValues();
                healthAndShields.Invincible = true;
                AreaManager.Instance.EndArea();
                break;
            case GameState.PAUSE:
                break;
            case GameState.WARPING:
                shipController.Warping = true;
                shipController.Freeze = false;
                shipController.StopThrust = false;
                shipController.AlignGunnerWithPilot();
                WarpEffectBehaviour.instance.StartWarp();
                //AreaManager.Instance.LoadNewArea();
                StartCoroutine(DelayedAreaLoad());
                break;
            case GameState.GAME_OVER:

                foreach (Player player in FindObjectsOfType<Player>())
                {
                    player.SetPlayerActionMap("MenuNavigation");
                }

                SceneManager.LoadScene("MainMenu");
                break;
        }
    }

    IEnumerator DelayedAreaLoad()
    {
        yield return new WaitForSeconds(3f);
        AreaManager.Instance.LoadNewArea();
        shipController.AlignGunnerWithPilot();
    }

    private void CheckForRespawn()
    {
        
        if (shipController && !shipController.gameObject.activeSelf)
        {
            SwitchState(GameState.RESPAWN);

            // TODO: Some effect for respawning
        }
    }

    private void RespawnPlayer()
    {
        respawnTime += Time.deltaTime;

        if (respawnTime >= MAX_RESPAWN_TIME)
        {
            respawnTime = 0;

            if (shipController && shipController.TryGetComponent(out PlayerRespawn playerRespawn))
            {
                playerRespawn.Respawn();
            }
        }
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
        }

        Instance = this;

        random = new System.Random(seed);

        shipController = GameObject.FindGameObjectWithTag("Player").GetComponent<ShipController>();
    }

    private void Start()
    {
        UpdateState();
    }

    private void Update()
    {
        switch (gameState)
        {
            case GameState.WARPING:
                break;
            case GameState.BATTLE:
                CheckForRespawn();
                break;
            case GameState.RESPAWN:
                RespawnPlayer();
                break;
            case GameState.BATTLE_END:
                break;
            case GameState.PAUSE:
                break;
            case GameState.GAME_OVER:
                break;
        }
    }

    public GameState GameState { get { return gameState; } }
    public System.Random Random { get { return random; } }
    public ShipController Player { get { return shipController; } }
}
