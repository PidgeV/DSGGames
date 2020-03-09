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

    [SerializeField] protected WaveBehaviour waveBehaviour;

    private Area area;

    [SerializeField] protected Transform[] spawnpoints;
    [SerializeField] protected Transform[] waypoints;

    // Start is called before the first frame update
    void Start()
    {
        TryGetComponent(out area);

        StartCoroutine(CreateNode());
    }

    private void Update()
    {
        if (GameManager.Instance.GameState == GameState.BATTLE)
        {
            if (AreaManager.Instance.EnemiesDead)
            {
                GameManager.Instance.SwitchState(GameState.BATTLE_END);
            }
        }
    }

    private IEnumerator CreateNode()
    {
        Transform spParent = new GameObject("Spawnpoints").transform;
        spParent.parent = transform;
        spParent.position = Vector3.zero;

        if (area && area.spawns && area.spawns.childCount > 0)
        {
            spawnpoints = area.Spawns;
        }
        else
        {
            GenerateSpawnpoints();
        }

        if (area && area.waypoints && area.waypoints.childCount > 0)
        {
            waypoints = area.Waypoints;
        }
        else
        {
            GenerateWaypoints();
        }

        yield return StartCoroutine(SpawnWave(0));
    }

    protected void GenerateSpawnpoints()
    {
        List<Transform> sp = new List<Transform>();

        for (int i = 0; i < waveBehaviour.GetMaxEnemyCount(0); i++)
        {
            Transform spawnpoint = new GameObject("Spawnpoint").transform;
            spawnpoint.parent = area.waypoints;

            Vector3 position = AreaManager.Instance.FindRandomPosition;

            if (!Physics.CheckSphere(position, 200, 1 << 8))
            {
                spawnpoint.position = position;

                sp.Add(spawnpoint);
            }
        }

        spawnpoints = sp.ToArray();
    }

    protected void GenerateWaypoints()
    {
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
    }

    protected GameObject SpawnEnemy(GameObject prefab, Vector3 spawnpoint, Vector3 destination)
    {
        GameObject enemy = Instantiate(prefab);

        enemy.transform.position = spawnpoint;

        if (enemy.TryGetComponent(out EnemyController enemyController))
        {
            enemyController.Spawn = spawnpoint;
            enemyController.SpawnDestination = destination;
            enemyController.waypoints = waypoints;
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

    protected virtual IEnumerator SpawnWave(int waveIndex, bool safeSpawn = true)
    {
        WaveSpawn spawnBehaviour = waveBehaviour.Waves[waveIndex];

        int spawnIndex = 0;

        for (int i = 0; i < spawnBehaviour.Spawns.Length; i++)
        {
            SpawnInfo spawnInfo = spawnBehaviour.Spawns[i];

            if (spawnInfo.type == EnemyType.SWARMER)
            {
                Transform spawnpoint = safeSpawn ? area.FindSafeSpawn() : spawnpoints[spawnIndex];

                GameObject swarmer = SpawnEnemy(swarmerPrefab, spawnpoint);

                if (swarmer.TryGetComponent(out Flock flock))
                {
                    flock.startingCount = spawnInfo.count;
                }

                if (!safeSpawn)
                    spawnIndex = (spawnIndex + 1) % spawnpoints.Length;
            }
            else
            {
                for (int j = 0; j < spawnInfo.count; j++)
                {
                    Transform spawnpoint = safeSpawn ? area.FindSafeSpawn() : spawnpoints[spawnIndex];

                    switch (spawnInfo.type)
                    {
                        case EnemyType.FIGHTER:
                            SpawnEnemy(fighterPrefab, spawnpoint);
                            break;
                        case EnemyType.CHARGER:
                            SpawnEnemy(chargerPrefab, spawnpoint);
                            break;
                        case EnemyType.CRUISER:
                            SpawnEnemy(cruiserPrefab, spawnpoint);
                            break;
                        case EnemyType.CARGO:
                            SpawnEnemy(cargoPrefab, spawnpoint);
                            break;
                    }

                    if (!safeSpawn)
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
