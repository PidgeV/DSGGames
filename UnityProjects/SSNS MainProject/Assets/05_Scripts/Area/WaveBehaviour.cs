using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Wave Behaviour", menuName = "Dreadnova/New Wave Behaviour")]
public class WaveBehaviour : ScriptableObject
{
    [SerializeField] private float timeBetweenWaves = 30;
    [SerializeField] private float timeBetweenCargoSpawns = 40;

    [SerializeField] private SpawnBehaviour[] waves;

    public int GetMaxEnemyCount(int waveIndex)
    {
        int count = 0;

        foreach(SpawnInfo spawn in waves[waveIndex].Spawns)
        {
            count += spawn.count;
        }

        return count;
    }

    public float TimeBetweenWaves { get { return timeBetweenWaves; } }
    public float TimeBetweenCargoSpawns { get { return timeBetweenCargoSpawns; } }
    public SpawnBehaviour[] Waves { get { return waves; } }
}
