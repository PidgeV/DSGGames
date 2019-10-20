using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomPosition : MonoBehaviour
{
    public float radius = 500; // Radiuas from world center
    // Start is called before the first frame update
    void Start()
    {
        Vector3 newPos = Vector3.zero;

        newPos.x = Random.Range(-radius, radius);
        newPos.y = Random.Range(-radius, radius);
        newPos.y = Random.Range(-radius, radius);

        transform.position = newPos;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
