using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Complete
{
    public class ChaserPatrolState : FSMState
    {
        private Player player;
        private ChaserController controller;
        private GameObject[] waypoints;
        private float distance;
        private float playerDist;
        private int patrolID = 0;
        private bool randomPoint;
        private bool sawPlayer = false;
        public ChaserPatrolState(ChaserController enemyController, Player playerObj, GameObject[] wayPoints, float waypointDistance, float playerDistance, bool randomizePoint = false)
        {
            controller = enemyController;
            player = playerObj;
            waypoints = wayPoints;
            distance = waypointDistance;
            playerDist = playerDistance;
            randomPoint = randomizePoint;
        }

        public override void Act()
        {

        }

        public override void EnterStateInit()
        {
            sawPlayer = false;
        }


        public override void Reason()
        {
            //Check distance to waypoint
            if (Vector3.Distance(controller.transform.position, waypoints[patrolID].transform.position) < distance)
            {
                if (randomPoint)
                {
                    patrolID = Random.Range(0, waypoints.Length);
                }
                else
                {
                    patrolID++;

                    if (patrolID > waypoints.Length) patrolID = 0;
                }
            }

            //Check distance to player
            if (Vector3.Distance(controller.transform.position, player.gameObject.transform.position) < playerDist)
            {
                if (sawPlayer)
                {
                    controller.PerformTransition(Transition.SawPlayer);
                }
            }
        }

        IEnumerator SeesPlayer()
        {
            bool leave = false;
            while (!leave)
            {
                yield return new WaitForSeconds(1f);
                Vector3 dir = player.transform.position - controller.transform.position;
                Ray ray = new Ray(controller.transform.position, dir);
                RaycastHit hitInfo;

                Physics.Raycast(ray, out hitInfo);

                if (hitInfo.collider.gameObject.Equals(player.gameObject))
                {
                    sawPlayer = true;
                    leave = true;
                }
            }
        }

        void AvoidObstacles(ref Vector3 dir)
        {
            RaycastHit hitInfo;

            //Check direction facing
            if (Physics.SphereCast(controller.transform.position, controller.raySize, controller.transform.forward.normalized, 
                out hitInfo, controller.collisionCheckDistance, controller.obstacleLayer) ||
                Physics.SphereCast(controller.transform.position, controller.raySize, controller.rbSelf.velocity.normalized, 
                out hitInfo, controller.collisionCheckDistance, controller.obstacleLayer))
            {
                // Get the desired direction we need to move to move around  the obstacle. Transform to world co-ordinates (gets the obstacleMoveDirection wrt the current foward direction).
                Vector3 turnDir = transform.TransformDirection(transform.right);
                turnDir.Normalize();

                dir += turnDir;
                obstacleHit = true;
            }
        }


    }
}