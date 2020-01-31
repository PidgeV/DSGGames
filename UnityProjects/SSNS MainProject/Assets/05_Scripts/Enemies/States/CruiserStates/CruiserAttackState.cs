using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Complete;


/// <summary>
/// The attack state for the Cruiser enemy
/// </summary>
public class CruiserAttackState : FSMState
{
	// Reference to the parent controller
	CruiserController myController;

	float stateCounter = 0.0f;
	float shotCounter = 0.0f;

	bool inAttackRange = false;

	public CruiserAttackState(CruiserController controller)
	{
		stateID = FSMStateID.Attacking;

		myController = controller;
		EnterStateInit();
	}

	/// <summary>
	/// Works as on enabled
	/// </summary>
	public override void EnterStateInit()
	{
		stateCounter = 0.0f;
		shotCounter = 0.0f;
	}

	/// <summary>
	/// Change State
	/// </summary>
	public override void Reason()
	{
		// Dead
		if (myController.HP <= 0)
		{
			myController.PerformTransition(Transition.NoHealth);
			return;
		}
		
		// Patroling
		if ((stateCounter += Time.deltaTime) > 0.25f)
		{
			if (myController.CheckPlayer(myController.LostRange) == false) {
				myController.PerformTransition(Transition.Patrol);
			}

			stateCounter = 0.0f;
		}
	}

	/// <summary>
	/// Works as Update
	/// </summary>
	public override void Act()
	{
		if (myController.player == null)
		{
			// Do nothing
			return;
		}
		else
		{
			Vector3 playerPos = myController.player.transform.position;
			Vector3 currentPos = myController.transform.position;

			Quaternion targetRotation = Quaternion.LookRotation(myController.player.transform.position - myController.transform.position);

			// Are we in range to shoot?
			inAttackRange = Vector3.Distance(playerPos, currentPos) < myController.AttackRange;
			inAttackRange = Quaternion.Angle(myController.transform.rotation, targetRotation) < 5;

			if ((shotCounter += Time.deltaTime) > 1f) {

				// If we are, we want to Aim at the player
				if (inAttackRange == true)
				{
					Shoot();
				}
				else
				{
					Vector3 direction = (myController.player.transform.position - myController.transform.position).normalized;

					float speed = 55f * 12f;

					myController.myRigidbody.AddForce(direction * speed * Time.deltaTime, ForceMode.Acceleration);
					myController.transform.rotation = Quaternion.Slerp(myController.transform.rotation, targetRotation, 0.01f);
				}

				shotCounter = 0.0f;
			}
		}
	}

	/// <summary>
	/// Aim and Shoot at the player
	/// </summary>
	public void Shoot()
	{
		Debug.Log("Shoot");
		if (myController.myState == CruiserState.Defensive)
		{
			// We are in normal mode

		}
		else
		{
			// We are in aggro mode

		}
	}
}
