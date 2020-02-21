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

    [HideInInspector] public ShipController shipController;

    [SerializeField] private GameState gameState = GameState.NODE_TRANSITION;

    [SerializeField] private int seed;

    private System.Random random;

    private const int MAX_RESPAWN_TIME = 5;

    private float respawnTime;

    private bool respawn;

	/// <summary>
	/// Pause the game and open the pause menu
	/// This is called from the players OnPause
	/// </summary>
	public void PauseGame()
	{
		Player[] players = GameObject.FindObjectsOfType<Player>();

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
                healthAndShields.Invincible = false;
                break;
            case GameState.BATTLE_END:
				healthAndShields.ResetValues();
                healthAndShields.Invincible = true;
                AreaManager.Instance.EndArea();
                break;
            case GameState.PAUSE:
                break;
            case GameState.NODE_SELECTION:
                shipController.StopThrust = true;
                NodeManager.Instance.BeginNodeSelection();
                break;
            case GameState.NODE_TRANSITION:
                shipController.StopThrust = false;
                AreaManager.Instance.LoadNewArea(NodeManager.Instance.CurrentNode.NodeInfo);
                break;
            case GameState.GAME_END:
                SceneManager.LoadScene("MainMenu");
                break;
        }
    }

    private void CheckForRespawn()
    {
        if (respawn)
        {
            respawnTime += Time.deltaTime;

            if (respawnTime >= MAX_RESPAWN_TIME)
            {
                respawnTime = 0;
                respawn = false;

                if (shipController && shipController.TryGetComponent(out PlayerRespawn playerRespawn))
                {
                    playerRespawn.Respawn();
                }
            }
        }
        else if (shipController && !shipController.gameObject.activeSelf)
        {
            respawn = true;

            // TODO: Some effect for respawning
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
    }

    private void Start()
    {
        UpdateState();

        Physics.IgnoreLayerCollision(13, 13); // cause projectiles to ignore projectiles
        Physics.IgnoreLayerCollision(12, 12); // cause shields to ignore shields
        Physics.IgnoreLayerCollision(8, 12); // cause obstacles to ignore shields
    }

    private void Update()
    {
        switch (gameState)
        {
            case GameState.NODE_TRANSITION:
                break;
            case GameState.BATTLE:
                CheckForRespawn();
                break;
            case GameState.BATTLE_END:
                break;
            case GameState.NODE_SELECTION:
                break;
            case GameState.PAUSE:
                break;
            case GameState.GAME_END:
                break;
        }
    }

    public GameState GameState { get { return gameState; } }

    public System.Random Random { get { return random; } }
}
