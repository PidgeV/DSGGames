using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnState : FSMState
{
    private DreadnovaSpawnInfo spawnInfo;

    private AdvancedFSM controller;

    public SpawnState(AdvancedFSM enemyController)
    {
        controller = enemyController;
        stateID = FSMStateID.Spawned;
    }

    public override void EnterStateInit()
    {
    }

    public override void Reason()
    {
        if (spawnInfo.spawn == null || IsInCurrentRange(controller.transform, spawnInfo.destination, 1))
        {
            spawnInfo.enemy = null;
            controller.PerformTransition(Transition.Patrol);
        }
    }

    public override void Act()
    {
        if (spawnInfo.spawn != null)
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

            //Move towards position. No need to worry about obstacles or 
            controller.transform.position = Vector3.MoveTowards(controller.transform.position, spawnInfo.destination, flockLeader.PatrolSpeed * Time.deltaTime);
        }
        else
        {
            //Calculate direction
            Vector3 direction = spawnInfo.destination - controller.transform.position;
            direction.Normalize();

            Vector3 newDir = Vector3.RotateTowards(controller.transform.forward, direction, 20 * Time.deltaTime, 0);
            Quaternion rot = Quaternion.LookRotation(newDir);
            controller.transform.rotation = Quaternion.Lerp(controller.transform.rotation, rot, 20 * Time.deltaTime);

            //Movement
            controller.GetComponent<Rigidbody>().AddForce(controller.transform.forward.normalized * 50, ForceMode.Acceleration);
        }

    }

    public DreadnovaSpawnInfo SpawnInfo { set { spawnInfo = value; spawnInfo.enemy = controller.gameObject; } }
}
