using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SNSSTypes;

[CreateAssetMenu(fileName = "Spawn Behaviour", menuName = "Dreadnova/New Spawn Behaviour")]
public class SpawnBehaviour : ScriptableObject
{
    [SerializeField] private SpawnInfo[] spawns;

    public SpawnInfo[] Spawns { get { return spawns; } }
}

[System.Serializable]
public struct SpawnInfo
{
    public EnemyType type;
    public int count;
    public int spawned;
}
