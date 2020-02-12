using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Wave Behaviour", menuName = "Dreadnova/New Wave Behaviour")]
public class WaveBehaviour : ScriptableObject
{
    public float timeBetweenWaves = 30;
    public bool loop = true;

    public delegate void SpawnedEventHandler(GameObject[] enemies);

    public SpawnedEventHandler EnemiesSpawned;

    [SerializeField] private SpawnBehaviour[] waves;

    private int waveIndex;

    private float waveTime;

    private void OnEnable()
    {
        waveIndex = 0;
    }

    /// <summary>
    /// Only call this function in an MonoBehaviour update function
    /// </summary>
    public void WaveUpdate(Transform[] waypoints)
    {
        waveTime += Time.deltaTime;

        if (waveTime >= timeBetweenWaves)
        {
            GameObject[] enemies = waves[waveIndex].SpawnEnemies(waypoints);

            EnemiesSpawned.Invoke(enemies);

            waveTime = 0;
            waveIndex++;
        }
    }
}
