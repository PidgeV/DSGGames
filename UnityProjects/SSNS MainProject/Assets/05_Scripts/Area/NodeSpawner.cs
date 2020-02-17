using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SNSSTypes;

/// <summary>
/// TODO: Make it spawn randomly
/// </summary>
public class NodeSpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    [SerializeField] protected GameObject fighterPrefab;
    [SerializeField] protected GameObject chargerPrefab;
    [SerializeField] protected GameObject swarmerPrefab;
    [SerializeField] protected GameObject cruiserPrefab;
    [SerializeField] protected GameObject cargoPrefab;

    [Space(10)]
    [SerializeField] protected Transform[] spawnpoints;
    [SerializeField] protected Transform[] waypoints;

    [SerializeField] protected WaveBehaviour waveBehaviour;

    private int spawnIndex;

    // Start is called before the first frame update
    void Start()
    {
        if (TryGetComponent(out AsteroidSpawner asteroidSpawner))
        {
            asteroidSpawner.minPos = -AreaManager.Instance.AreaSize * 1.2f;
            asteroidSpawner.maxPos = AreaManager.Instance.AreaSize * 1.2f;
            asteroidSpawner.maxAsteroids = NodeManager.Instance.CurrentNode.Event.asteroidAmount;
            asteroidSpawner.maxScale = AreaManager.Instance.AreaSize / 200;
        }

        StartCoroutine(CreateNode());
    }

    private void Update()
    {
        if (GameManager.Instance.GameState == GameState.BATTLE)
        {
            if (AreaManager.Instance.EnemiesDead)
            {
                AreaManager.Instance.EndArea();
            }
        }
    }

    private IEnumerator CreateNode()
    {
        Transform spParent = new GameObject("Spawnpoints").transform;
        spParent.parent = transform;
        spParent.position = Vector3.zero;

        List<Transform> sp = new List<Transform>();

        for (int i = 0; i < waveBehaviour.GetMaxEnemyCount(0); i++)
        {
            Transform spawnpoint = new GameObject("Spawnpoint").transform;
            spawnpoint.parent = spParent;

            Vector3 position = AreaManager.Instance.FindRandomPosition;

            if (!Physics.CheckSphere(position, 200, 1 << 8))
            {
                spawnpoint.position = position;

                sp.Add(spawnpoint);
            }
        }

        spawnpoints = sp.ToArray();

        Transform wpParent = new GameObject("Waypoints").transform;
        wpParent.parent = transform;
        wpParent.position = Vector3.zero;

        List<Transform> wp = new List<Transform>();

        for (int i = 0; i < Random.Range(5, 15); i++)
        {
            Transform waypoint = new GameObject("Waypoint").transform;
            waypoint.parent = wpParent;

            Vector3 position = AreaManager.Instance.FindRandomPosition;

            if (!Physics.CheckSphere(position, 200, 1 << 8))
            {
                waypoint.position = position;

                wp.Add(waypoint);
            }
        }

        waypoints = wp.ToArray();

        yield return StartCoroutine(SpawnWave(0));

        AreaManager.Instance.OnSpawnFinished();
    }

    protected GameObject SpawnEnemy(GameObject prefab, Vector3 spawnpoint, Vector3 destination)
    {
        GameObject enemy = Instantiate(prefab);

        enemy.transform.position = spawnpoint;

        if (enemy.TryGetComponent(out AdvancedFSM advancedFSM))
        {
            advancedFSM.spawnpoint = spawnpoint;
            advancedFSM.spawnDestination = destination;
            advancedFSM.waypoints = waypoints;
        }
        else if (enemy.TryGetComponent(out Flock flock))
        {
            flock.SetSpawnDestination = spawnpoint;
            flock.SetSpawnDestination = destination;
            flock.WayPoints = waypoints;
        }

        AreaManager.Instance.OnObjectAdd(enemy, true);

        return enemy;
    }

    protected virtual GameObject SpawnEnemy(GameObject prefab, Transform spawnpoint)
    {
        GameObject enemy = SpawnEnemy(prefab, spawnpoint.position, spawnpoint.position);

        return enemy;
    }

    protected virtual IEnumerator SpawnWave(int waveIndex)
    {
        WaveSpawn spawnBehaviour = waveBehaviour.Waves[waveIndex];

        for (int i = 0; i < spawnBehaviour.Spawns.Length; i++)
        {
            SpawnInfo spawnInfo = spawnBehaviour.Spawns[i];

            if (spawnInfo.type == EnemyType.SWARMER)
            {
                GameObject swarmer = SpawnEnemy(swarmerPrefab, spawnpoints[spawnIndex]);

                if (swarmer.TryGetComponent(out Flock flock))
                {
                    flock.startingCount = spawnInfo.count;
                }
            }
            else
            {
                for (int j = 0; j < spawnInfo.count; j++)
                {
                    switch (spawnInfo.type)
                    {
                        case EnemyType.FIGHTER:
                            SpawnEnemy(fighterPrefab, spawnpoints[spawnIndex]);
                            break;
                        case EnemyType.CHARGER:
                            SpawnEnemy(chargerPrefab, spawnpoints[spawnIndex]);
                            break;
                        case EnemyType.CRUISER:
                            SpawnEnemy(cruiserPrefab, spawnpoints[spawnIndex]);
                            break;
                        case EnemyType.CARGO:
                            SpawnEnemy(cargoPrefab, spawnpoints[spawnIndex]);
                            break;
                    }

                    spawnIndex = (spawnIndex + 1) % spawnpoints.Length;

                    yield return new WaitForSeconds(waveBehaviour.TimeBetweenEnemySpawns);
                }
            }
        }
    }

    protected virtual void DrawGizmos()
    {
        if (spawnpoints != null)
        {
            foreach (Transform sp in spawnpoints)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(sp.position, 4.0f);
            }
        }

        if (waypoints != null)
        {
            Gizmos.color = Color.blue;

            foreach (Transform wp in waypoints)
            {
                Gizmos.DrawWireSphere(wp.position, 4.0f);
            }
        }
    }

    private void OnDrawGizmos()
    {
        DrawGizmos();
    }
}
