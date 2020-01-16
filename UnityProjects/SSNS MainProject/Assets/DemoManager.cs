using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Keybindings:
/// 
/// Number keys are to switch between cameras
/// M to spawn charger enemy
/// N to spawn fighter enemy
/// </summary>
public class DemoManager : MonoBehaviour
{
    Camera enabledCam;
    [Tooltip("Can use up to 10 cameras")]
    [SerializeField] Camera[] cameras;
    [SerializeField] GameObject chargerPrefab;
    [SerializeField] GameObject fighterPrefab;
    [SerializeField] Transform enemySpawnLocation;

    [SerializeField] GameObject playerObj;
    GameObject spawnedEnemy;

    // Start is called before the first frame update
    void Start()
    {
        enabledCam = cameras[0];
        foreach (Camera c in cameras)
        {
            c.enabled = false;
        }
        enabledCam.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        ChangeCamera();
        SpawnEnemy();
    }

    void ChangeCamera()
    {
        foreach (KeyCode vKey in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKey(vKey))
            {
                //your code here
                var test = vKey;

                if (test.ToString().Length == 6)
                {
                    string strTest = test.ToString().Remove(0, 5);
                    int camNum = int.Parse(strTest);

                    if (camNum >= 0 && camNum < cameras.Length)
                    {
                        enabledCam.enabled = false;
                        enabledCam = cameras[camNum];
                        enabledCam.enabled = true;
                    }
                }
            }
        }
    }

    void SpawnEnemy()
    {

        if (Input.GetKeyDown(KeyCode.M) && chargerPrefab != null)
        {
            if (enemySpawnLocation != null)
            {
                if (spawnedEnemy != null)
                {
                    Destroy(spawnedEnemy);
                }
                spawnedEnemy = Instantiate(chargerPrefab, enemySpawnLocation.position, enemySpawnLocation.rotation);
            }
            else
            {
                Debug.LogError("Spawn location missing. Please give Demo Manager an enemy spawn location.");
            }
        }
        else if (Input.GetKeyDown(KeyCode.N) && fighterPrefab != null)
        {
            if (enemySpawnLocation != null)
            {
                if (spawnedEnemy != null)
                {
                    Destroy(spawnedEnemy);
                }
                spawnedEnemy = Instantiate(fighterPrefab, enemySpawnLocation.position, enemySpawnLocation.rotation);
            }
            else
            {
                Debug.LogError("Spawn location missing. Please give Demo Manager an enemy spawn location.");
            }
        }
    }
}
