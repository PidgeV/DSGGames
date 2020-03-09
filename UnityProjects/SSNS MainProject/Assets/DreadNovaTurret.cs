using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DreadNovaTurret : MonoBehaviour
{
	public Transform Target;

	public bool HasTarget { get { return Target != null; } }

	private void Start()
	{
		Target = GameObject.FindGameObjectWithTag("Player").transform;
	}

	// Update is called once per frame
	void Update()
	{
		if (Target == null) return;

		float minRotation = -45;
		float maxRotation = 10;

		Vector3 rot = Vector3.RotateTowards(transform.forward, Target.position - transform.position, 0.5f * Time.deltaTime, 0.0f);
		transform.rotation = Quaternion.LookRotation(rot, transform.up);

		Vector3 currentRotation = transform.localRotation.eulerAngles;

		if (currentRotation.x < 360 + minRotation && currentRotation.x > 180) {
			currentRotation.x = 360 + minRotation;
		}

		if (currentRotation.x > maxRotation && currentRotation.x < 180) {
			currentRotation.x = maxRotation;
		}

		currentRotation.z = 0;

		transform.localRotation = Quaternion.Euler(currentRotation);

		Vector3 toPosition = (Target.position - transform.position).normalized;
		float angleToPosition = Vector3.Angle(transform.forward, toPosition);

		if (angleToPosition < 5)
		{
			print("It worked");
		}
	}

	// Draw the Gizmos
	void OnDrawGizmos()
	{
		if (Target == null) return;

		Gizmos.DrawLine(transform.position, Target.position);
	}
}
