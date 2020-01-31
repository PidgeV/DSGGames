using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Complete;


/// <summary>
/// The attack state for the Cruiser enemy
/// </summary>
public class CruiserPatrolState : FSMState
{
	// Reference to the parent controller
	CruiserController myController;

	// The points to go to when Im patrolling
	List<Vector3> patrolPoints = new List<Vector3>();

	#region Members

	float counter = 0.0f;
	int patrolID = 0;

	bool randomPoint;

	//Obstacle variables
	Vector3 obstacleAvoidDirection = Vector3.right;
	bool obstacleHit = false;
	float obstacleTimer = 0;
	float avoidTime = 2f;

	#endregion

	/// <summary>
	/// Constructor
	/// </summary>
	public CruiserPatrolState(CruiserController controller)
	{
		stateID = FSMStateID.Patrolling;

		myController = controller;
		EnterStateInit();
	}

	/// <summary>
	/// Works as on enabled
	/// </summary>
	public override void EnterStateInit()
	{
		patrolPoints = new List<Vector3>();

		int pointCount = 15;
		for (int count = 0; count < pointCount; count++)
		{
			// NOTE -- The patroll points are based on the area around the cruiser when the cruiser enemy enters the CruiserPatrolState

			// Get a random position
			float x = Random.Range(-500, 500);
			float y = Random.Range(-200, 200);
			float z = Random.Range(-500, 500);

			// Add it to our patrol points
			patrolPoints.Add(myController.transform.position + new Vector3(x, y, z));
		}

		counter = 0.0f;
	}

	/// <summary>
	/// Change State
	/// </summary>
	public override void Reason()
	{
		// Death
		if (myController.HP <= 0) {
			myController.PerformTransition(Transition.NoHealth);
			return;
		}

		// Saw Player
		if ((counter += Time.deltaTime) > 0.25f)
		{
			if (myController.CheckPlayer(myController.DetectionRange) == true) {
				myController.PerformTransition(Transition.SawPlayer);
			}

			counter = 0.0f;
		}

		// If we're in range of the next patrol point, we move to the next one
		if (Vector3.Distance(myController.transform.position, patrolPoints[patrolID]) < 20)
		{
			if (randomPoint)
			{
				patrolID = Random.Range(0, patrolPoints.Count);
			}
			else
			{
				patrolID++;

                Debug.Log("Changing States");

				if (patrolID >= patrolPoints.Count) patrolID = 0;
			}
		}
	}

	/// <summary>
	/// Works as Update
	/// </summary>
	public override void Act()
	{
		// If the point we're trying to go to does NOT exist
		if (patrolPoints[patrolID] == null)
		{
			return;
		}
		else
		{
			// Set the direction we're moving
			Vector3 direction = myController.transform.forward.normalized;

			// Run that direction through a filter to avoid obsticles
			AvoidObstacles(ref direction);


			// If we haven't hit an obstacle and the time is at 0
			if (!obstacleHit && obstacleTimer == 0)
			{
				// sets desired direction to target intercept point
				direction = patrolPoints[patrolID] - myController.transform.position;

				// We use this to compare the any adjusted angles from Avoid Obstacles
				Vector3 forward = myController.transform.forward;
				
				Quaternion rotation = Quaternion.LookRotation(Vector3.RotateTowards(forward, direction, myController.regRotationForce * Time.deltaTime, 0));

				myController.transform.rotation = Quaternion.Slerp(myController.transform.rotation, rotation, Time.deltaTime);
			}
			// Else we want to increment the timer and move towards the target direction
			else
			{
				//if obstacles, ignore desired direction and move to the right of obstacles
				obstacleTimer += Time.deltaTime;
				if (obstacleTimer > avoidTime)
				{
					obstacleTimer = 0;
					obstacleHit = false;
				}
				
				Quaternion rotation = Quaternion.LookRotation(Vector3.RotateTowards(myController.transform.forward, direction, myController.regRotationForce * Time.deltaTime, 0));

				myController.transform.rotation = Quaternion.Slerp(myController.transform.rotation, rotation, Time.deltaTime);
			}

			// Movement
			// Move regular speed if obstacle is in the way or player is not the target
			myController.myRigidbody.AddForce(myController.transform.forward.normalized * myController.Acceleration, ForceMode.Acceleration);
		}
	}

	/// <summary>
	/// Adjust the movement direction for obstacles
	/// </summary>
	private void AvoidObstacles(ref Vector3 dir)
	{
		RaycastHit hitInfo;

		//Check direction facing
		if (Physics.SphereCast(myController.transform.position, myController.raySize, myController.transform.forward.normalized, out hitInfo, myController.collisionCheckDistance, myController.obstacleLayer) || 
			Physics.SphereCast(myController.transform.position, myController.raySize, myController.myRigidbody.velocity.normalized, out hitInfo, myController.collisionCheckDistance, myController.obstacleLayer))
		{
			// Get the desired direction we need to move to move around  the obstacle. Transform to world co-ordinates (gets the obstacleMoveDirection wrt the current foward direction).
			Vector3 turnDir = myController.transform.TransformDirection(hitInfo.normal + Vector3.right);
			turnDir.Normalize();

			dir += turnDir;
			obstacleHit = true;
		}
	}
}
