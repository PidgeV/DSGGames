using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwarmLeaderPatrolState : FSMState
{
    FlockLeaderController controller;
    Flock swarm;

    private int patrolID = 0;
    private bool randomPoint;
    public SwarmLeaderPatrolState(FlockLeaderController leader, Flock swarmObj, bool randomize = true)
    {
        controller = leader;
        randomPoint = randomize;
        swarm = swarmObj;
        swarm.swarmFollowRadius = controller.PatrolRadius;

        if (randomPoint) patrolID = Random.Range(0, controller.waypoints.Length);


        stateID = FSMStateID.Patrolling;
    }

    public override void Act()
    {
        Move();
    }

    public override void Reason()
    {
        if(swarm.defenseTarget != null)
        {
            //Enter defend state mode
            controller.PerformTransition(Transition.Defend);
        }

        if(swarm.player != null)
        {
            //Check distance to player
            if (Vector3.Distance(controller.transform.position, swarm.player.transform.position) <= controller.PlayerDistance) 
            {
                if (AIManager.aiManager.CanAttack(controller.aiType))
                {
                    controller.PerformTransition(Transition.Attack);
                }
            }
        }
        else
        {
            swarm.player = GameObject.FindGameObjectWithTag("Player");
        }

        //Check waypoint distance
        if (controller.waypoints.Length > 0)
        {
            if (Vector3.Distance(controller.transform.position, controller.waypoints[patrolID].transform.position) <= controller.WaypointDistance) //Check distance to current waypoint
            {
                if (randomPoint)
                {
                    patrolID = Random.Range(0, controller.waypoints.Length); //Choose random patrol point
                }
                else
                {
                    patrolID++;//Progress to next waypoint

                    if (patrolID >= controller.waypoints.Length) patrolID = 0; //Circle back to first waypoint
                }
            }
        }
    }

    public override void EnterStateInit()
    {
        //Do this when entering the state
        swarm.swarmFollowRadius = controller.PatrolRadius;
    }

    void Move()
    {
        if (controller.waypoints.Length > 0 && controller.waypoints[patrolID] != null)
        {
            //Move towards position. No need to worry about obstacles or 
            controller.transform.position = Vector3.MoveTowards(controller.transform.position, controller.waypoints[patrolID].transform.position, controller.Stats.shipSpeed * Time.deltaTime);
        }
    }
}