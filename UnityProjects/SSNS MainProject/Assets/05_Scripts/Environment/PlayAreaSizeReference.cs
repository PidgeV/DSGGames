using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAreaSizeReference : MonoBehaviour
{
	public float size = 5000f;

	private void OnDrawGizmos()
	{
		Gizmos.DrawWireSphere(transform.position, size);
	}
}
