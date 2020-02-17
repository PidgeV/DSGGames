using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnState : FSMState
{
    private AdvancedFSM controller;

    public SpawnState(AdvancedFSM enemyController)
    {
        controller = enemyController;
        stateID = FSMStateID.Spawned;
    }

    public override void EnterStateInit()
    {
        if (controller.GetType() == typeof(Complete.FlockLeaderController))
        {
            ChangeColliders(controller.transform.parent.gameObject, false);
        }
        else
        {
            ChangeColliders(controller.gameObject, false);
        }

    }

    public override void Reason()
    {
        if (IsInCurrentRange(controller.transform, controller.spawnDestination, 50))
        {
            if (controller.GetType() == typeof(Complete.FlockLeaderController))
            {
                ChangeColliders(controller.transform.parent.gameObject, true);

                controller.PerformTransition(Transition.Defend);
            }
            else
            {
                ChangeColliders(controller.gameObject, true);

                controller.PerformTransition(Transition.Patrol);
            }
        }
    }

    public override void Act()
    {
        if (controller.spawnpoint != null)
        {
            Move();
        }
    }

    //Moves
    void Move()
    {

        if (controller.GetType() == typeof(Complete.FlockLeaderController))
        {
            Complete.FlockLeaderController flockLeader = (Complete.FlockLeaderController)controller;

            //Calculate direction
            Vector3 direction = controller.spawnDestination - controller.transform.position;
            direction.Normalize();

            Vector3 newDir = Vector3.RotateTowards(controller.transform.forward, direction, 20 * Time.deltaTime, 0);
            Quaternion rot = Quaternion.LookRotation(newDir);
            controller.transform.rotation = Quaternion.Lerp(controller.transform.rotation, rot, 20 * Time.deltaTime);

            //Move towards position. No need to worry about obstacles or 
            controller.transform.position += controller.transform.forward * flockLeader.PatrolSpeed * Time.deltaTime;
        }
        else
        {
            //Calculate direction
            Vector3 direction = controller.spawnDestination - controller.transform.position;
            direction.Normalize();

            Vector3 newDir = Vector3.RotateTowards(controller.transform.forward, direction, 20 * Time.deltaTime, 0);
            Quaternion rot = Quaternion.LookRotation(newDir);
            controller.transform.rotation = Quaternion.Lerp(controller.transform.rotation, rot, 20 * Time.deltaTime);

            //Movement
            controller.GetComponent<Rigidbody>().AddForce(controller.transform.forward.normalized * 50, ForceMode.Acceleration);
        }

    }

    private void ChangeColliders(GameObject parent, bool active)
    {
        foreach (Collider collider in parent.GetComponentsInChildren<Collider>())
        {
            collider.enabled = active;
        }
    }
}
