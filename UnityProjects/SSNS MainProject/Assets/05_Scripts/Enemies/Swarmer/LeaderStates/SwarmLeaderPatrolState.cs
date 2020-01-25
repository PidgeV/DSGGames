using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Complete
{
    public class SwarmLeaderPatrolState : FSMState
    {
        FlockLeaderController controller;
        Flock swarm;
        GameObject player;
        GameObject[] waypoints;

        private int patrolID = 0;
        private bool randomPoint;
        public SwarmLeaderPatrolState(FlockLeaderController leader, Flock flockObj, GameObject[] wayPoints, bool randomize = true)
        {
            controller = leader;
            swarm = flockObj;
            waypoints = wayPoints;
            randomPoint = randomize;

            if (randomPoint) patrolID = Random.Range(0, waypoints.Length);

            stateID = FSMStateID.Patrolling;
        }

        public override void Act()
        {
            Move();
        }

        public override void Reason()
        {
            if(player != null)
            {
                //Check distance to player
                if (Vector3.Distance(controller.transform.position, player.transform.position) <= controller.PlayerDistance) 
                {
                    //controller.PerformTransition(Transition.SawPlayer);
                }
            }
            else
            {
                player = GameObject.FindGameObjectWithTag("Player");
            }

            //Check waypoint distance
            if(Vector3.Distance(controller.transform.position, waypoints[patrolID].transform.position) <= controller.WaypointDistance) //Check distance to current waypoint
            {
                if (randomPoint)
                {
                    patrolID = Random.Range(0, waypoints.Length); //Choose random patrol point
                }
                else
                {
                    patrolID++;//Progress to next waypoint

                    if (patrolID >= waypoints.Length) patrolID = 0; //Circle back to first waypoint
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
            if (waypoints[patrolID] != null)
            {
                //Move towards position. No need to worry about obstacles or 
                controller.transform.position = Vector3.MoveTowards(controller.transform.position, waypoints[patrolID].transform.position, controller.Speed * Time.deltaTime);
            }
            else
            {
                Debug.LogError("Null reference with waypoints.");
            }
        }
    }
}