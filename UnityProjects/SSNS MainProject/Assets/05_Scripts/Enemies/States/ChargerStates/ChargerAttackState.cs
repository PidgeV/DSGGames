using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Complete
{
    public class ChargerAttackState : FSMState
    {
        ChaserController controller;
        private Player player;
        Vector3 interceptPoint;
        float dotProduct;

        //Obstacle variables
        Vector3 obstacleAvoidDirection = Vector3.right;
        bool obstacleHit = false;
        float obstacleTimer = 0;
        float avoidTime = 2f;

        //constructor
        public ChargerAttackState(ChaserController enemyController, Player playerObj)
        {
            controller = enemyController;
            player = playerObj;

            stateID = FSMStateID.Attacking;
        }

        public override void Act()
        {
            CalculateIntercept();
            Move();
        }

        public override void Reason()
        {

        }

        public override void EnterStateInit()
        {

        }

        //Calculates the intercept point
        private void CalculateIntercept()
        {
            Rigidbody rbTarget = player.gameObject.GetComponent<Rigidbody>();
            //positions
            Vector3 targetPosition = player.transform.position;
            //velocities
            Vector3 velocity = controller.rbSelf ? controller.rbSelf.velocity : Vector3.zero;
            Vector3 targetVelocity = rbTarget ? rbTarget.velocity : Vector3.zero;

            //calculate intercept
            interceptPoint = InterceptCalculationClass.FirstOrderIntercept(controller.transform.position, velocity, controller.rbSelf.velocity.magnitude, targetPosition, targetVelocity);
        }

        //Moves
        void Move()
        {
            if (interceptPoint != null)
            {
                //Calculate direction
                Vector3 direction = controller.transform.forward; // sets forward
                direction.Normalize();

                AvoidObstacles(ref direction); // will change direction towards the right if an obstacle is in the way

                //Rotation
                if (!obstacleHit && obstacleTimer == 0)
                {
                    direction = interceptPoint - controller.transform.position; // sets desired direction to target intercept point

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
<<<<<<< HEAD
            if (Physics.SphereCast(controller.transform.position, controller.RaySize, controller.transform.forward.normalized, out hitInfo, controller.CollisionCheckDistance, controller.ObstacleLayer))// ||
                //Physics.SphereCast(controller.transform.position, controller.RaySize, controller.rbSelf.velocity.normalized, out hitInfo, controller.CollisionCheckDistance, controller.ObstacleLayer))
=======
            if (Physics.SphereCast(controller.transform.position, controller.RaySize, controller.transform.forward.normalized,
                out hitInfo, controller.CollisionCheckDistance, controller.ObstacleLayer) ||
                Physics.SphereCast(controller.transform.position, controller.RaySize, controller.rbSelf.velocity.normalized,
                out hitInfo, controller.CollisionCheckDistance, controller.ObstacleLayer))
>>>>>>> parent of c3e206489... Merge branch 'Trixie-Test'
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