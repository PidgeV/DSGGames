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
    [SerializeField] private GameObject swarmerPrefab;

    private AsteroidSpawner asteroidSpawner;

    private void Awake()
    {
        asteroidSpawner = GetComponent<AsteroidSpawner>();

        if (asteroidSpawner)
        {
            asteroidSpawner.minPos = -AreaManager.Instance.AreaSize * 1.2f;
            asteroidSpawner.maxPos = AreaManager.Instance.AreaSize * 1.2f;
            asteroidSpawner.maxAsteroids = AreaManager.Instance.AreaSize / 3;
            asteroidSpawner.maxScale = AreaManager.Instance.AreaSize / 200;
        }
    }

    // Start is called before the first frame update
    IEnumerator Start()
    {
        if (GameManager.Instance)
        {
            System.Random random = GameManager.Instance.Random;

            for (int j = 0; j < random.Next(2, 10); j++)
            {
                GameObject enemy = Instantiate<GameObject>(fighterPrefab);
                enemy.transform.position = transform.position + Vector3.one * random.Next((int)(-AreaManager.Instance.AreaSize / 1.5f), (int)(AreaManager.Instance.AreaSize / 1.5f));
                enemy.transform.parent = transform;

                FighterController controller = enemy.GetComponent<FighterController>();
                controller.waypoints = new GameObject[] { gameObject };

                AreaManager.Instance.OnObjectAdd(enemy, true);

                yield return null;
            }

            for (int j = 0; j < random.Next(2, 10); j++)
            {
                GameObject enemy = Instantiate<GameObject>(chaserPrefab);
                enemy.transform.position = transform.position + Vector3.one * random.Next((int)(-AreaManager.Instance.AreaSize / 1.5f), (int)(AreaManager.Instance.AreaSize / 1.5f));
                enemy.transform.parent = transform;

                ChargerController controller = enemy.GetComponent<ChargerController>();
                controller.waypoints = new GameObject[] { gameObject };

                AreaManager.Instance.OnObjectAdd(enemy, true);

                yield return null;
            }

            for (int j = 0; j < random.Next(5); j++)
            {
                GameObject enemy = Instantiate<GameObject>(swarmerPrefab);
                enemy.transform.position = transform.position + Vector3.one * random.Next((int)(-AreaManager.Instance.AreaSize / 1.5f), (int)(AreaManager.Instance.AreaSize / 1.5f));
                enemy.transform.parent = transform;

                Flock controller = enemy.GetComponent<Flock>();
                controller.startingCount = random.Next(20, 50);

                if (asteroidSpawner != null)
                    controller.WayPoints = asteroidSpawner.asteroids;
                else
                    controller.WayPoints = new GameObject[] { gameObject };

                AreaManager.Instance.OnObjectAdd(enemy, true);

                yield return null;
            }

            AreaManager.Instance.OnSpawnFinished();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance && GameManager.Instance.GameState == SNSSTypes.GameState.BATTLE && AreaManager.Instance.EnemiesDead)
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
