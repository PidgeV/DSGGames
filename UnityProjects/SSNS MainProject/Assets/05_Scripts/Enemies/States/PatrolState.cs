using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Complete
{
    public class PatrolState : FSMState
    {
        private Player player;
        private AdvancedFSM controller;
        private GameObject[] waypoints;
        private float distance;
        private float playerDist;
        private int patrolID = 0;
        public PatrolState(AdvancedFSM enemyController, Player playerObj, GameObject[] wayPoints, float waypointDistance, float playerDistance)
        {
            controller = enemyController;
            player = playerObj;
            waypoints = wayPoints;
            distance = waypointDistance;
            playerDist = playerDistance;
        }

        public override void Act()
        {
           
        }

        public override void Reason()
        {
            //Check distance to waypoint
            if(Vector3.Distance(controller.transform.position, waypoints[patrolID].transform.position) < distance)
            {
                patrolID++;

                if (patrolID > waypoints.Length) patrolID = 0;
            }

            //Check distance to player
            if (Vector3.Distance(controller.transform.position, player.gameObject.transform.position) < playerDist)
            {
                controller.PerformTransition(Transition.SawPlayer);
            }
        }
    }
}