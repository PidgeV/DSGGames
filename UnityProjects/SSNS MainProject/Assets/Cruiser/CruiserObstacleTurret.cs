using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CruiserObstacleTurret : MonoBehaviour
{
	public Transform Target;
	
	[SerializeField] private CruiserTargetArea targetArea;
	[SerializeField] private Collider obstacleArea;

	// Update is called once per frame
	void Update()
	{
		if (Target == null)
		{
			GameObject target = targetArea.GetObstacle();
			if (target) Target = target.transform;
		}
		else
		{
			float minRotation = -45;
			float maxRotation = 10;

			Vector3 rot = Vector3.RotateTowards(transform.forward, Target.position - transform.position, 2f * Time.deltaTime, 0.0f);
			transform.rotation = Quaternion.LookRotation(rot, transform.up);

			Vector3 currentRotation = transform.localRotation.eulerAngles;

			if (currentRotation.x < 360 + minRotation && currentRotation.x > 180)
			{
				currentRotation.x = 360 + minRotation;
			}

			if (currentRotation.x > maxRotation && currentRotation.x < 180)
			{
				currentRotation.x = maxRotation;
			}

			currentRotation.z = 0;

			transform.localRotation = Quaternion.Euler(currentRotation);

			Vector3 toPosition = (Target.position - transform.position).normalized;
			float angleToPosition = Vector3.Angle(transform.forward, toPosition);

			if (angleToPosition < 5)
			{ 
				if (coRunning == false) {
					StartCoroutine(coShoot());
				}
			}
		}
	}

	bool coRunning = false;
	IEnumerator coShoot()
	{
		coRunning = true;
		yield return new WaitForSeconds(1.5f);
		if (Target)
		{
			Destroy(Target.gameObject);
		}
		coRunning = false;
	}

	// Draw the Gizmos
	void OnDrawGizmos()
	{
		if (Target == null) return;

		Gizmos.DrawLine(transform.position, Target.position);
	}
}
