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
[RequireComponent(typeof(DreadnovaController))]
public class DreadnovaSpawner : MonoBehaviour
{
    public delegate void EnemySpawned(GameObject enemy, int spawnIndex);

    [Header("Enemy Prefabs")]
    [SerializeField] private GameObject fighterPrefab;
    [SerializeField] private GameObject chargerPrefab;
    [SerializeField] private GameObject swarmerPrefab;
    [SerializeField] private GameObject cruiserPrefab;
    [SerializeField] private GameObject cargoPrefab;

    [SerializeField] private WaveBehaviour shieldWaveBehaviour;
    [SerializeField] private WaveBehaviour attackWaveBehaviour;

    [SerializeField] private Transform[] spawnpoints;
    [SerializeField] private Transform[] waypoints;

    [SerializeField] private float distanceToTravelAway = 50;

    private DreadnovaController dreadnovaController;

    private CargoController cargoController;

    private WaveBehaviour currentWaveBehaviour;

    private float timeBetweenEnemySpawns = 0.4f;

    private float waveTime;
    private int waveCount;

    private float cargoTime;

    private int spawnIndex;

    // Start is called before the first frame update
    void Start()
    {
        AreaManager.Instance.OnSpawnFinished();

        waveTime = shieldWaveBehaviour.TimeBetweenWaves;

        TryGetComponent(out dreadnovaController);

        dreadnovaController.StateChanged += DreadnovaStateChange;
    }

    private void Update()
    {
        if (GameManager.Instance.GameState == GameState.BATTLE)
        {
            if (AreaManager.Instance.EnemyCount <= currentWaveBehaviour.GetMaxEnemyCount(waveCount))
            {
                waveTime += Time.deltaTime;

                if (waveTime >= currentWaveBehaviour.TimeBetweenWaves)
                {
                    waveTime = 0;
                    waveCount = (waveCount + 1) % currentWaveBehaviour.Waves.Length;

                    StartCoroutine(SpawnWave());
                }
            }

            if (!cargoController)
            {
                cargoTime += Time.deltaTime;

                if (cargoTime >= currentWaveBehaviour.TimeBetweenCargoSpawns)
                {
                    SpawnCargo();
                }
            }
        }
    }

    private void DreadnovaStateChange(FSMStateID stateID)
    {
        if (stateID == FSMStateID.Defend)
        {
            currentWaveBehaviour = shieldWaveBehaviour;
        }
        else if (stateID == FSMStateID.Attacking)
        {
            currentWaveBehaviour = attackWaveBehaviour;
        }

        if (!cargoController) SpawnCargo();
    }

    private void SpawnCargo()
    {
        Vector3 spawnpoint = AreaManager.Instance.FindRandomPosition;
        Vector3 swarmerSpawnpoint = spawnpoint + Random.insideUnitSphere * 20;

        GameObject cargo = SpawnEnemy(cargoPrefab, spawnpoint, spawnpoint);
        GameObject swarmer = SpawnEnemy(swarmerPrefab, swarmerSpawnpoint, swarmerSpawnpoint);

        cargo.TryGetComponent(out cargoController);

        if (swarmer.TryGetComponent(out Flock flock))
        {
            flock.defenseTarget = cargo;
        }

        cargoTime = 0;
    }

    protected IEnumerator SpawnWave()
    {
        SpawnBehaviour spawnBehaviour = shieldWaveBehaviour.Waves[waveCount];

        for (int i = 0; i < spawnBehaviour.Spawns.Length; i++)
        {
            SpawnInfo spawnInfo = spawnBehaviour.Spawns[i];

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
                    case EnemyType.SWARMER:
                        SpawnEnemy(swarmerPrefab, spawnpoints[spawnIndex]);
                        break;
                    case EnemyType.CRUISER:
                        SpawnEnemy(cruiserPrefab, spawnpoints[spawnIndex]);
                        break;
                }

                spawnIndex = (spawnIndex + 1) % spawnpoints.Length;

                yield return new WaitForSeconds(timeBetweenEnemySpawns);
            }
        }

        yield return null;
    }

    private GameObject SpawnEnemy(GameObject prefab, Transform spawnpoint)
    {
        GameObject enemy = SpawnEnemy(prefab, spawnpoint.position, spawnpoint.position + spawnpoint.forward * distanceToTravelAway);

        enemy.transform.rotation = spawnpoint.rotation;

        return enemy;
    }

    private GameObject SpawnEnemy(GameObject prefab, Vector3 spawnpoint, Vector3 destination)
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

    public WaveBehaviour Wave { get { return shieldWaveBehaviour; } }
}