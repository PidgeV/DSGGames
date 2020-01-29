using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manager that handles each area (node)
/// Allows loading for new areas and destroys the old area
/// </summary>
public class AreaManager : MonoBehaviour
{
    public static AreaManager Instance;

    [SerializeField] private int areaSize;
    [SerializeField] private Area lastArea;
    [SerializeField] private Area currentArea;

    private bool nextAreaLoaded;
    private bool lastAreaDestroyed;
    private bool areaEnded;

    /// <summary>
    /// Ends the area giving the ship the node's reward 
    /// and waits for the UI to be hidden again before allowing next node selection
    /// </summary>
    public void EndArea()
    {
        areaEnded = true;

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
            location = lastArea.location + Vector3.forward * 10000
        };

        // Spawns all prefabs
        foreach (GameObject go in nodeInfo.nodeEvent.prefabsToSpawn)
        {
            GameObject spawnedGO = Instantiate<GameObject>(go);
            spawnedGO.transform.parent = currentArea.parent;
            spawnedGO.transform.position = currentArea.location;
        }

        // Disables the last area
        if (lastArea != null && lastArea.parent != null)
            lastArea.parent.gameObject.SetActive(false);

        StartCoroutine(DestroyLastArea());
    }

    /// <summary>
    /// Destroys the last area while loading the next one
    /// </summary>
    /// <returns></returns>
    private IEnumerator DestroyLastArea()
    {
        // Destroys the objects in the last area
        foreach(GameObject go in lastArea.objects)
        {
            Destroy(go);

            yield return null;
        }

        // Destroys the parent of the area
        if (lastArea != null && lastArea.parent != null)
            Destroy(lastArea.parent.gameObject);

        lastArea = null;

        lastAreaDestroyed = true;
    }

    /// <summary>
    /// Adds object to the area
    /// TODO: Disable enemy AI until player arrives at area
    /// </summary>
    /// <param name="gObject">The object that is needed to be added to the area</param>
    /// <param name="enemy">Whether it's an enemy or not</param>
    public void OnObjectAdd(GameObject gObject, bool enemy = false)
    {
        if (enemy)
        {
            gObject.transform.parent = currentArea.enemiesParent;
            currentArea.enemies.Add(gObject);
        }
        else
        {
            gObject.transform.parent = currentArea.obstacleParent;
            currentArea.objects.Add(gObject);
        }
    }

    /// <summary>
    /// Kills all enemeis in the area
    /// </summary>
    public void KillEnemies()
    {
        foreach(GameObject enemy in currentArea.enemies.ToArray())
        {
            HealthAndShields health = enemy.GetComponent<HealthAndShields>();
            health.TakeDamage(100000000, 100000000);
        }
    }

    /// <summary>
    /// Removes enemy from area
    /// </summary>
    /// <param name="enemy">The enemy to remove</param>
    public void OnEnemyDeath(GameObject enemy)
    {
        currentArea.enemies.Remove(enemy);
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

            NodeManager.Instance.ArrivedNextNode();
            GameManager.Instance.shipController.transform.position = currentArea.location;
            SkyboxManager.Instance.SwitchToSkybox(NodeManager.Instance.CurrentNode.Skybox);
            GameManager.Instance.SwitchState(SNSSTypes.GameState.BATTLE);
        }
    }

    public int AreaSize { get { return areaSize; } }
    public bool EnemiesDead { get { return currentArea.enemies == null || currentArea.enemies.Count == 0; } }

    /// <summary>
    /// Class that stores all the area info
    /// </summary>
    [System.Serializable]
    class Area
    {
        public Transform parent;
        public Transform obstacleParent;
        public Transform enemiesParent;
        public Vector3 location;
        public List<GameObject> objects;
        public List<GameObject> enemies;

        public Area(string name)
        {
            parent = new GameObject("Area: " + name).transform;
            obstacleParent = new GameObject("Obstacles").transform;
            obstacleParent.parent = parent;
            enemiesParent = new GameObject("Enemies").transform;
            enemiesParent.parent = parent;
            objects = new List<GameObject>();
            enemies = new List<GameObject>();
        }

        ~Area()
        {
            parent = null;
            location = Vector3.zero;
            objects = null;
            enemies = null;
            System.GC.EndNoGCRegion();
        }
    }
}
