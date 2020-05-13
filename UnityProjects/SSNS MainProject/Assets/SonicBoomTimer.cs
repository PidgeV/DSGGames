using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SonicBoomTimer : MonoBehaviour
{
    [SerializeField] float[] frames;
    [SerializeField] GameObject SonicBoomPrefab;

    float count = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        foreach(float f in frames)
        {
            if (count == f)
            {
                Instantiate(SonicBoomPrefab, transform);
                Debug.Log("Sonic Boom Spawned");
            }
        }

        count++;
        Debug.Log("Frame: " + count);
    }
}
