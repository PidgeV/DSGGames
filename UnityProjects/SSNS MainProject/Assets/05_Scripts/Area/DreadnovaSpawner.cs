using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SNSSTypes;

[System.Serializable]
public class DreadnovaSpawner : MonoBehaviour
{
    [SerializeField] private DreadnovaState state;

    [SerializeField] private WaveBehaviour waveBehaviour;

    [SerializeField] private Transform[] spawnpoints;
    [SerializeField] private Transform[] waypoints;

    private void EnemiesSpawned(GameObject[] enemies)
    {
        int radius = 100;

        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].transform.position = spawnpoints[Random.Range(0, spawnpoints.Length)].position + Random.insideUnitSphere * radius;
        }
    }

    private void Awake()
    {
        waveBehaviour.EnemiesSpawned += EnemiesSpawned;
    }

    // Start is called before the first frame update
    void Start()
    {
        AreaManager.Instance.OnSpawnFinished();
    }

    // Update is called once per frame
    void Update()
    {
        waveBehaviour.WaveUpdate(waypoints);
    }

    private void OnDrawGizmos()
    {
        if (spawnpoints != null)
        {
            Gizmos.color = Color.green;

            foreach (Transform sp in spawnpoints)
            {
                Gizmos.DrawWireSphere(sp.position, 4.0f);
            }
        }

        if (waypoints != null)
        {
            Gizmos.color = Color.blue;

            foreach (Transform sp in waypoints)
            {
                Gizmos.DrawWireSphere(sp.position, 4.0f);
            }
        }
    }
}
