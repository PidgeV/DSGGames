using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Wave Behaviour", menuName = "Dreadnova/New Wave Behaviour")]
public class WaveBehaviour : ScriptableObject
{
    [SerializeField] private float timeBetweenWaves = 30;
    [SerializeField] private bool loop = true;
    [SerializeField] private bool random = false;

    [SerializeField] private SpawnBehaviour[] waves;

    private int waveIndex;

    public float TimeBetweenWaves { get { return timeBetweenWaves; } }
    public bool Loop { get { return loop; } }
    public bool Random { get { return random; } }
    public SpawnBehaviour[] Waves { get { return waves; } }
}
