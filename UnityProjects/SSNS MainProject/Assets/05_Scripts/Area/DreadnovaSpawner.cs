using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SNSSTypes;

/// <summary>
/// TODO: 
/// - All ships when spawned need to travel to a point away from the ship
///     - Spawnpoints have a gameobject child that points away from the ship
///     - All enemies have a state for when they spawn which
///     - They will travel to this point then switch to patrol state
/// - Ships need to disable their colliders when spawned, preventing them from hurting themselves
///     - Probably inside the spawn state
/// </summary>
[System.Serializable]
public class DreadnovaSpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    [SerializeField] private GameObject fighterPrefab;
    [SerializeField] private GameObject chargerPrefab;
    [SerializeField] private GameObject swarmerPrefab;
    [SerializeField] private GameObject cruiserPrefab;
    [SerializeField] private GameObject cargoPrefab;

    [SerializeField] private WaveBehaviour waveBehaviour;

    [SerializeField] private Transform[] spawnpoints;
    [SerializeField] private Transform[] waypoints;

    [SerializeField] private DreadnovaState state;

    [SerializeField] private float distanceToTravelAway = 100;

    private float waveTime;
    private int waveCount;

    private DreadnovaSpawnInfo[] dreadnovaSpawns;

    private DreadnovaSpawnInfo[] FindUnseenSpawnpoints()
    {
        // TODO: Determine spawnpoints that aren't seen by the player
        List<DreadnovaSpawnInfo> spawns = new List<DreadnovaSpawnInfo>();

        foreach (DreadnovaSpawnInfo info in dreadnovaSpawns)
        {
            if (!info.enemy)
            {
                spawns.Add(info);
            }
        }

        return spawns.ToArray();
    }

    private void Awake()
    {
        dreadnovaSpawns = new DreadnovaSpawnInfo[spawnpoints.Length];

        for (int i = 0; i < spawnpoints.Length; i++)
        {
            dreadnovaSpawns[i].spawn = spawnpoints[i];
            dreadnovaSpawns[i].destination = spawnpoints[i].position + spawnpoints[i].forward * distanceToTravelAway;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        AreaManager.Instance.OnSpawnFinished();

        waveTime = waveBehaviour.TimeBetweenWaves;
    }

    private void Update()
    {
        waveTime += Time.deltaTime;

        if (waveTime >= waveBehaviour.TimeBetweenWaves)
        {
            waveTime = 0;
            waveCount = (waveCount + 1) % waveBehaviour.Waves.Length;

            StartCoroutine(SpawnWave());
        }
    }

    protected IEnumerator SpawnWave()
    {
        SpawnBehaviour spawnBehaviour = waveBehaviour.Waves[waveCount];

        for (int i = 0; i < spawnBehaviour.Spawns.Length; i++)
        {
            SpawnInfo spawnInfo = spawnBehaviour.Spawns[i];

            for (int j = 0; j < spawnInfo.count; j++)
            {
                DreadnovaSpawnInfo[] availableSpawnpoints;
                while (true)
                {
                    availableSpawnpoints = FindUnseenSpawnpoints();

                    if (availableSpawnpoints != null && availableSpawnpoints.Length > 0) break;

                    yield return new WaitForSeconds(0.2f);
                }

                DreadnovaSpawnInfo spawn = availableSpawnpoints[Random.Range(0, availableSpawnpoints.Length)];

                switch (spawnInfo.type)
                {
                    case EnemyType.FIGHTER:
                        SpawnEnemy(fighterPrefab, spawn);
                        break;
                    case EnemyType.CHARGER:
                        SpawnEnemy(chargerPrefab, spawn);
                        break;
                    case EnemyType.SWARMER:
                        SpawnEnemy(swarmerPrefab, spawn);
                        break;
                    case EnemyType.CRUISER:
                        SpawnEnemy(cruiserPrefab, spawn);
                        break;
                    case EnemyType.CARGO:
                        SpawnEnemy(cargoPrefab, spawn);
                        break;
                }
            }
        }
    }

    private void SpawnEnemy(GameObject prefab, DreadnovaSpawnInfo spawnInfo)
    {

        GameObject enemy = Instantiate(prefab);

        AdvancedFSM advancedFSM = enemy.GetComponent<AdvancedFSM>();

        if (advancedFSM.CurrentState.ID == FSMStateID.Spawned)
        {
            SpawnState state = (SpawnState)advancedFSM.CurrentState;

            state.SpawnInfo = spawnInfo;
        }
    }

    private void OnDrawGizmos()
    {
        if (spawnpoints != null)
        {

            foreach (Transform sp in spawnpoints)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(sp.position, 4.0f);

                Gizmos.color = Color.red;
                Gizmos.DrawRay(sp.position, sp.forward * distanceToTravelAway);
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

    public WaveBehaviour Wave { get { return waveBehaviour; } }
}

public struct DreadnovaSpawnInfo
{
    public Transform spawn;
    public Vector3 destination;
    public GameObject enemy;

    public bool Taken { get { return enemy; } }
}