using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Test spawner
/// </summary>
public class testAreaSpawner : MonoBehaviour
{
    [SerializeField] private GameObject fighterPrefab;
    [SerializeField] private GameObject chaserPrefab;

    private AsteroidSpawner asteroidSpawner;

    private void Awake()
    {
        asteroidSpawner = GetComponent<AsteroidSpawner>();

        if (asteroidSpawner)
        {
            asteroidSpawner.minPos = -AreaManager.Instance.AreaSize;
            asteroidSpawner.maxPos = AreaManager.Instance.AreaSize;
        }
    }

    // Start is called before the first frame update
    IEnumerator Start()
    {
        System.Random random = GameManager.Instance.Random;

        for (int j = 0; j < random.Next(2, 10); j++)
        {
            GameObject enemy = Instantiate<GameObject>(fighterPrefab);
            enemy.transform.position = transform.position + Vector3.one * random.Next(100, AreaManager.Instance.AreaSize / 2);
            enemy.transform.parent = transform;

            Complete.FighterController controller = enemy.GetComponent<Complete.FighterController>();
            controller.waypoints = new GameObject[] { gameObject };

            AreaManager.Instance.OnObjectAdd(enemy, true);

            yield return null;
        }

        for (int j = 0; j < random.Next(2, 10); j++)
        {
            GameObject enemy = Instantiate<GameObject>(chaserPrefab);
            enemy.transform.position = transform.position + Vector3.one * random.Next(100, AreaManager.Instance.AreaSize / 2);
            enemy.transform.parent = transform;

            Complete.ChaserController controller = enemy.GetComponent<Complete.ChaserController>();
            controller.waypoints = new GameObject[] { gameObject };

            AreaManager.Instance.OnObjectAdd(enemy, true);

            yield return null;
        }

        AreaManager.Instance.OnSpawnFinished();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.GameState == SNSSTypes.GameState.BATTLE && AreaManager.Instance.EnemiesDead)
        {
            GameManager.Instance.SwitchState(SNSSTypes.GameState.BATTLE_END);
        }
    }

    private void OnDrawGizmos()
    {
        if (AreaManager.Instance)
            Gizmos.DrawWireSphere(transform.position, AreaManager.Instance.AreaSize);
    }
}
