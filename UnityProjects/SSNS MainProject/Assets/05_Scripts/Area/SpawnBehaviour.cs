using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SNSSTypes;

[CreateAssetMenu(fileName = "Spawn Behaviour", menuName = "Dreadnova/New Spawn Behaviour")]
public class SpawnBehaviour : ScriptableObject
{
    [SerializeField] private GameObject fighterPrefab;
    [SerializeField] private GameObject chargerPrefab;
    [SerializeField] private GameObject swarmerPrefab;
    [SerializeField] private GameObject cruiserPrefab;
    [SerializeField] private GameObject cargoPrefab;

    [SerializeField] private SpawnInfo[] spawns;

    public GameObject[] SpawnEnemies(Transform[] waypoints)
    {
        List<GameObject> enemies = new List<GameObject>();

        for (int i = 0; i < spawns.Length; i++)
        {
            GameObject prefab = null;

            switch (spawns[i].type)
            {
                case EnemyType.FIGHTER:
                    prefab = fighterPrefab;
                    break;
                case EnemyType.CHARGER:
                    prefab = chargerPrefab;
                    break;
                case EnemyType.SWARMER:
                    prefab = swarmerPrefab;
                    break;
                case EnemyType.CRUISER:
                    prefab = cruiserPrefab;
                    break;
                case EnemyType.CARGO:
                    prefab = cargoPrefab;
                    break;
            }

            GameObject enemy = Instantiate(prefab);
            
            if (enemy.TryGetComponent(out AdvancedFSM fsm))
            {
                fsm.waypoints = waypoints;
            }
            else if (enemy.TryGetComponent(out Flock flock))
            {
                flock.WayPoints = waypoints;
            }

            AreaManager.Instance.OnObjectAdd(enemy, true);

            enemies.Add(enemy);
        }

        return enemies.ToArray();
    }

    [System.Serializable]
    private struct SpawnInfo
    {
        public EnemyType type;
        public int count;
    }
}
