using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SNSSTypes;

public class DreadnovaSpawner : AreaSpawner
{
    [SerializeField] private WaveBehaviour shieldWaveBehaviour;
    [SerializeField] private WaveBehaviour attackWaveBehaviour;

    [SerializeField] private float distanceToTravelAway = 200;

    private CargoController cargoController;

    // Start is called before the first frame update
    void Start()
    {
        waveBehaviour = shieldWaveBehaviour;

        Transform dreadnova = transform.Find("Dreadnova");

        if (TryGetComponent(out DreadnovaController controller))
        {
            controller.StateChanged += DreadnovaStateChange;
        }
    }

    private void Update()
    {
        // Leave empty
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

    public void SpawnCargo()
    {
        Transform spawnpoint = AreaManager.Instance.CurrentArea.FindSafeSpawn("CargoSpawn");

        Vector3 swarmerSpawnpoint = spawnpoint.position + Random.insideUnitSphere * 20;

        GameObject cargo = SpawnEnemy(cargoPrefab, spawnpoint.position, spawnpoint.position);
        GameObject swarmer = SpawnEnemy(swarmerPrefab, swarmerSpawnpoint, swarmerSpawnpoint);

        cargo.TryGetComponent(out cargoController);

        if (swarmer.TryGetComponent(out Flock flock))
        {
            flock.defenseTarget = cargo;
            flock.startingCount = 100;

            //flock.FlockLeader.gameObject.transform.position = swarmerSpawnpoint;
        }

        DialogueSystem.Instance.AddDialogue(2);
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

    public WaveBehaviour Wave { get { return waveBehaviour; } }
    public bool CargoExists { get { return cargoController; } }
}