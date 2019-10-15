using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetMove : MonoBehaviour
{
	public Transform target;

	// Update is called once per frame
	void Update()
	{
		transform.RotateAround(target.position, Vector3.forward, 20 * Time.deltaTime);
	}
}
