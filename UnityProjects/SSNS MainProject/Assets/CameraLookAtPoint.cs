using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLookAtPoint : MonoBehaviour
{
	public Transform point;

    // Update is called once per frame
    void Update()
    {
		transform.LookAt(point.position); 
    }
}
