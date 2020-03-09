using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SNSSTypes;

public class DreadnovaSpawner : NodeSpawner
{
    [SerializeField] private WaveBehaviour shieldWaveBehaviour;
    [SerializeField] private WaveBehaviour attackWaveBehaviour;

    [SerializeField] private float distanceToTravelAway = 200;

    [SerializeField] private DreadnovaController dreadnovaController;

    private CargoController cargoController;

    private float waveTime;
    private int waveCount;

    private float cargoTime;

    // Start is called before the first frame update
    void Start()
    {
        waveTime = shieldWaveBehaviour.TimeBetweenWaves;

        dreadnovaController.StateChanged += DreadnovaStateChange;

        waveBehaviour = shieldWaveBehaviour;
    }

    private void Update()
    {
        if (GameManager.Instance.GameState == GameState.BATTLE)
        {
            if (dreadnovaController.CurrentStateID == FSMStateID.Dead || dreadnovaController.CurrentStateID == FSMStateID.Running)
            {
                AreaManager.Instance.EndArea();
                enabled = false;
            }
            else
            {
                if (AreaManager.Instance.EnemyCount <= waveBehaviour.GetMaxEnemyCount(waveCount))
                {
                    waveTime += Time.deltaTime;

                    if (waveTime >= waveBehaviour.TimeBetweenWaves)
                    {
                        waveTime = 0;
                        waveCount = (waveCount + 1) % waveBehaviour.Waves.Length;

                        StartCoroutine(SpawnWave(waveCount, false));
                    }
                }

                if (!cargoController)
                {
                    cargoTime += Time.deltaTime;

                    if (cargoTime >= waveBehaviour.TimeBetweenCargoSpawns)
                    {
                        SpawnCargo();
                    }
                }
            }
        }
    }

    private void DreadnovaStateChange(FSMStateID stateID)
    {
        if (stateID == FSMStateID.Defend)
        {
            waveBehaviour = shieldWaveBehaviour;
        }
        else if (stateID == FSMStateID.Attacking)
        {
            waveBehaviour = attackWaveBehaviour;
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
            flock.startingCount = 100;
        }

        cargoTime = 0;
    }

    protected override GameObject SpawnEnemy(GameObject prefab, Transform spawnpoint)
    {
        GameObject enemy = SpawnEnemy(prefab, spawnpoint.position, spawnpoint.position + spawnpoint.forward * distanceToTravelAway);

        enemy.transform.rotation = spawnpoint.rotation;

        return enemy;
    }

    protected override void DrawGizmos()
    {
        base.DrawGizmos();

        if (spawnpoints != null)
        {
            foreach (Transform sp in spawnpoints)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawRay(sp.position, sp.forward * distanceToTravelAway);
            }
        }
    }

    public WaveBehaviour Wave { get { return shieldWaveBehaviour; } }
}