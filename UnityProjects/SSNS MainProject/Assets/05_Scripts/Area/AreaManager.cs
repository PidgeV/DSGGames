﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SNSSTypes;

/// <summary>
/// Manager that handles each area (node)
/// Allows loading for new areas and destroys the old area
/// </summary>
public class AreaManager : MonoBehaviour
{
    public static AreaManager Instance;

    public delegate void AreaLoadEventHandler();

    public event AreaLoadEventHandler AreaLoaded;

    private const int MIN_TRAVEL_TIME = 10;

    [SerializeField] private Area lastArea;
    [SerializeField] private Area currentArea;
    [SerializeField] private GameObject areaEffectPrefab;

    private AreaState state;

    private GameObject areaEffect;

    private bool nextAreaLoaded;
    private bool lastAreaDestroyed;
    private bool areaEnded;

    private float transitionTime;

    /// <summary>
    /// Ends the area giving the ship the node's reward 
    /// and waits for the UI to be hidden again before allowing next node selection
    /// </summary>
    public void EndArea()
    {
        areaEnded = true;

        //Destroy(areaEffect);

        // Grabs current reward and uses it on the ship
        Reward reward = NodeManager.Instance.CurrentNode.Reward;
        reward.UseReward(GameManager.Instance.shipController.myStats);

        // Updates the ui for the reward
        RewardManager.Instance.rewardUI.UpdateUI(reward);
    }

    /// <summary>
    /// Loads the area given. Uses the NodeEvent script to spawn 
    /// prefabs that should handle spawning objects in the area.
    /// </summary>
    /// <param name="nodeInfo">The node to load</param>
    public void LoadNewArea(NodeInfo nodeInfo)
    {
        // Moves player to a spot away from any areas
        // TODO: Implement effects for scene transition
        GameManager.Instance.shipController.transform.position = Vector3.forward * -10000;

        // Creates new area storing the old one
        lastArea = currentArea;
        currentArea = new Area(nodeInfo.name)
        {
            location = lastArea.location + Vector3.forward * 10000,
            size = nodeInfo.nodeEvent.areaSize
        };

        // Spawns all prefabs
        foreach (GameObject go in nodeInfo.nodeEvent.prefabsToSpawn)
        {
            GameObject spawnedGO = Instantiate(go);
            spawnedGO.transform.parent = currentArea.parent;
            spawnedGO.transform.position = currentArea.location;

            //Adjust smoke to random variable for now
            if(spawnedGO.TryGetComponent(out AdjustParticleSpaceSmoke smoke))
            {
                Vector3 size = new Vector3(Instance.AreaSize * 1.1f, Instance.AreaSize * 1.1f, Instance.AreaSize * 1.1f);
                smoke.ChangeSize(size);
                smoke.ChangeCapacity((int)(Mathf.Sqrt(size.x * size.y * size.z) / 500));
            }
        }

        // Disables the last area
        if (lastArea != null && lastArea.parent != null)
            lastArea.parent.gameObject.SetActive(false);

        StartCoroutine(TransitionAreas());
        StartCoroutine(DestroyLastArea());
    }

    /// <summary>
    /// Destroys the last area while loading the next one
    /// </summary>
    /// <returns></returns>
    private IEnumerator DestroyLastArea()
    {
        yield return null;

        // Destroys the parent of the area
        if (lastArea != null && lastArea.parent != null)
            Destroy(lastArea.parent.gameObject);

        lastArea = null;

        lastAreaDestroyed = true;
    }

    /// <summary>
    /// Transitions between areas
    /// </summary>
    /// <returns></returns>
    private IEnumerator TransitionAreas()
    {
        //areaEffect = GameObject.Instantiate(areaEffectPrefab, currentArea.location, Quaternion.identity);
        //areaEffect.transform.localScale = Vector3.one * currentArea.size * 2;

        yield return new WaitForSeconds(MIN_TRAVEL_TIME);
    }

    /// <summary>
    /// Adds object to the area
    /// </summary>
    /// <param name="gObject">The object that is needed to be added to the area</param>
    /// <param name="enemy">Whether it's an enemy or not</param>
    public void OnObjectAdd(GameObject gObject, bool enemy = false)
    {
        if (enemy)
        {
            gObject.transform.parent = currentArea.enemies;
        }
        else
        {
            gObject.transform.parent = currentArea.obstacles;
        }
    }

    /// <summary>
    /// Kills all enemeis in the area
    /// </summary>
    public void KillEnemies()
    {
        foreach(GameObject enemy in currentArea.enemies)
        {
            HealthAndShields healthAndShields = enemy.GetComponent<HealthAndShields>();

            if (healthAndShields)
                healthAndShields.TakeDamage(10000000, 1000000);
        }
    }

    /// <summary>
    /// Transitions to next area
    /// </summary>
    public void OnSpawnFinished()
    {
        nextAreaLoaded = true;
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
        }

        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        // Waits for reward UI to hide before switching to node selection
        if (areaEnded && !RewardManager.Instance.rewardUI.Shown)
        {
            areaEnded = false;
            GameManager.Instance.SwitchState(SNSSTypes.GameState.NODE_SELECTION);
        }
        // Transition to next node
        else if (nextAreaLoaded && lastAreaDestroyed)
        {
            nextAreaLoaded = false;
            lastAreaDestroyed = false;

            AreaLoaded?.Invoke();

            currentArea.enemies.gameObject.SetActive(true);
            currentArea.obstacles.gameObject.SetActive(true);

            // TODO: Should be in the testShipController
            GameManager.Instance.shipController.transform.position = PlayerDestination;
            GameManager.Instance.shipController.transform.rotation = Quaternion.Euler(0, 0, 0);

            SkyboxManager.Instance.SwitchToSkybox(NodeManager.Instance.CurrentNode.Skybox);
            GameManager.Instance.SwitchState(SNSSTypes.GameState.BATTLE);
        }
    }

	//
	private void OnDrawGizmos()
	{
		Gizmos.DrawWireSphere(currentArea.location, currentArea.size);
	}

	public int AreaSize { get { return currentArea.size; } }
    public bool EnemiesDead { get { return currentArea.enemies.childCount == 0; } }

    public Vector3 PlayerDestination { get { return currentArea.location - Vector3.forward * currentArea.size; } }
}

/// <summary>
/// Class that stores all the area info
/// </summary>
[System.Serializable]
public class Area
{
    public Transform parent;
    public Transform obstacles;
    public Transform enemies;
    public Vector3 location;
    public int size;

    public Area(string name)
    {
        parent = new GameObject("Area: " + name).transform;
        obstacles = new GameObject("Obstacles").transform;
        obstacles.parent = parent;
        obstacles.gameObject.SetActive(false);
        enemies = new GameObject("Enemies").transform;
        enemies.parent = parent;
        enemies.gameObject.SetActive(false);
    }

    ~Area()
    {
        parent = null;
        obstacles = null;
        enemies = null;
        location = Vector3.zero;
        size = 0;
        System.GC.EndNoGCRegion();
    }
}
