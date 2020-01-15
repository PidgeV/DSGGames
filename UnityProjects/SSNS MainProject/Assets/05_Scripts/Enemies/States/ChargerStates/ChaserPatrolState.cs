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

        //Obstacle variables
        Vector3 obstacleAvoidDirection = Vector3.right;
        bool obstacleHit = false;
        float obstacleTimer = 0;
        float avoidTime = 2f;

        //Constructor
        public ChaserPatrolState(ChaserController enemyController, Player playerObj, GameObject[] wayPoints, float waypointDistance, float playerDistance, bool randomizePoint = false)
        {
            controller = enemyController;
            player = playerObj;
            waypoints = wayPoints;
            distance = waypointDistance;
            playerDist = playerDistance;
            randomPoint = randomizePoint;
            stateID = FSMStateID.Patrolling;

            EnterStateInit();
        }

        //Do this always
        public override void Act()
        {
            Move();
        }

        //Initialize on entering state
        public override void EnterStateInit()
        {

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
                if (PlayerInVision())
                {
<<<<<<< HEAD
<<<<<<< HEAD
                    //Debug.DrawLine(controller.transform.position, player.transform.position);

                    if (timer >= timeAfterTransition)
                    {
                        timer = 0f;
                        if (PlayerInVision()) // in vision and has been patrolling for minimum time. This is to prevent the ai staying in attack mode and acting weird
                        {
                            controller.PerformTransition(Transition.SawPlayer);
                        }
                    }
=======
                    controller.PerformTransition(Transition.SawPlayer);
>>>>>>> parent of c3e206489... Merge branch 'Trixie-Test'
=======
                    controller.PerformTransition(Transition.SawPlayer);
>>>>>>> parent of c3e206489... Merge branch 'Trixie-Test'
                }
            }
            //Else dead transition to dead
            else if (controller.Health <= 0)
            {
                controller.PerformTransition(Transition.NoHealth);
            }
        }

        private bool PlayerInVision()
        {
            Vector3 dir = player.transform.position - controller.transform.position;
            Ray ray = new Ray(controller.transform.position, dir);
            RaycastHit hitInfo;

            Physics.Raycast(ray, out hitInfo);

            if (hitInfo.collider.gameObject.Equals(player.gameObject))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //Moves
        void Move()
        {
            if (waypoints[patrolID] != null)
            {
                //Calculate direction
                Vector3 direction = controller.transform.forward; // sets forward
                direction.Normalize();

                AvoidObstacles(ref direction); // will change direction towards the right if an obstacle is in the way

                //Rotation
                if (!obstacleHit && obstacleTimer == 0)
                {
                    direction = waypoints[patrolID].transform.position - controller.transform.position; // sets desired direction to target intercept point

                    Vector3 newDir = Vector3.RotateTowards(controller.transform.forward, direction, controller.RegRotationForce * Time.deltaTime, 0);
                    Quaternion rot = Quaternion.LookRotation(newDir);
                    controller.transform.rotation = Quaternion.Slerp(controller.transform.rotation, rot, Time.deltaTime);
                }
                else
                {
                    //if obstacles, ignore desired direction and move to the right of obstacles
                    obstacleTimer += Time.deltaTime;
                    if (obstacleTimer > avoidTime)
                    {
                        obstacleTimer = 0;
                        obstacleHit = false;
                    }

                    Vector3 newDir = Vector3.RotateTowards(controller.transform.forward, direction, controller.RegRotationForce * Time.deltaTime, 0);
                    Quaternion rot = Quaternion.LookRotation(newDir);
                    controller.transform.rotation = Quaternion.Slerp(controller.transform.rotation, rot, Time.deltaTime);
                }

                //Movement
                controller.rbSelf.AddForce(controller.transform.forward.normalized * controller.Acceleration, ForceMode.Acceleration); // move regular speed if obstacle is in the way or player is not target
            }
        }

        void AvoidObstacles(ref Vector3 dir)
        {
            RaycastHit hitInfo;

            //Check direction facing
            if (Physics.SphereCast(controller.transform.position, controller.RaySize, controller.transform.forward.normalized,
                out hitInfo, controller.CollisionCheckDistance, controller.ObstacleLayer))// ||
                //Physics.SphereCast(controller.transform.position, controller.RaySize, controller.rbSelf.velocity.normalized,
                //out hitInfo, controller.CollisionCheckDistance, controller.ObstacleLayer))
            {
                // Get the desired direction we need to move to move around  the obstacle. Transform to world co-ordinates (gets the obstacleMoveDirection wrt the current foward direction).
                Vector3 turnDir = controller.transform.TransformDirection(hitInfo.normal + Vector3.right);
                turnDir.Normalize();

                dir += turnDir;
                obstacleHit = true;
            }
        }
    }
}