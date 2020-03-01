using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapAddOnSpawn : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameObject.FindGameObjectWithTag("MinimapCamera").GetComponent<MiniMapDistanceColor>().Add(gameObject);
    }

    private void OnDestroy()
    {
        if (GameObject.FindGameObjectWithTag("MinimapCamera"))
            GameObject.FindGameObjectWithTag("MinimapCamera").GetComponent<MiniMapDistanceColor>().Remove(gameObject);
    }
}
