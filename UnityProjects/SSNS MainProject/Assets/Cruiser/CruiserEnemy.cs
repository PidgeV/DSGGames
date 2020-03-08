using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]


// TODO: 
// 4 turrets
// lightning attack
// small dudes 
// run from player
// sicko mode
// defensive state

public class CruiserEnemy : MonoBehaviour
{
	private Animator _animator;

	private float speed = 50;

	public List<Vector3> Waypoints { get; private set; }

	public float TargetDistanceToPoint { get; private set; }
	public float AggroRange { get; private set; }
	public float EscapeRange { get; private set; }

	// Start is called before the first frame update
	private void Awake()
	{
		_animator = GetComponent<Animator>();

		TargetDistanceToPoint = 50f;
		AggroRange = 1000f;
		EscapeRange = 1500f;
	}

	// Start is called before the first frame update
	private void Start()
	{
		Waypoints = AIUtilities.GenerateWaypoints(transform.position, 2000, 25);

		_animator.SetTrigger("Patrolling");
	}

	// Draw Gizmos each frame
	private void OnDrawGizmos()
	{
		if (Application.isPlaying == false) return;

		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, AggroRange);

		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere(transform.position, EscapeRange);

		foreach (Vector3 point in Waypoints)
		{
			Gizmos.color = Color.green;
			Gizmos.DrawSphere(point, 5);
		}
	}

	// Move and Rotate the cruiser ship
	public void Move(Vector3 newPosition)
	{
		Vector3 rot = Vector3.RotateTowards(transform.forward, newPosition - transform.position, 0.35f * Time.deltaTime, 0.0f);
		transform.rotation = Quaternion.LookRotation(rot, Vector3.up);

		transform.Translate(Vector3.forward * Time.deltaTime * speed);
	}
}

public static class AIUtilities
{
	public static List<Vector3> GenerateWaypoints(Vector3 center, float radius, int wpCount = 15)
	{
		List<Vector3> points = new List<Vector3>();

		for (int count = 0; count < wpCount; count++)
		{
			float x = Random.Range(-0.5f, 0.5f);
			float y = Random.Range(-0.5f, 0.5f);
			float z = Random.Range(-0.5f, 0.5f);

			Vector3 direction = new Vector3(x, y, z) * Random.Range(radius - (radius / 5f), radius);

			points.Add(center + direction);
		}

		return points;
	}
}
