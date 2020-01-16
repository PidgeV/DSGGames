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
    [SerializeField] Camera[] cameras;
    [SerializeField] GameObject chargerPrefab;
    [SerializeField] GameObject fighterPrefab;

    [SerializeField] GameObject playerObj;
    GameObject spawnedEnemy;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //if(Input.GetKeyDown("1") || Input.GetKeyDown("2") || Input.GetKeyDown("3") || Input.GetKeyDown("4") || Input.GetKeyDown("5"))
        //{

        //}
        foreach (KeyCode vKey in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKey(vKey))
            {
                //your code here
                var test = vKey;
            }
        }
    }
}
