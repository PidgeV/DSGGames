using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Complete
{
    public class FighterAttackState : FSMState
    {
        private Player player;
        private FighterController controller;
        Vector3 interceptPoint;
        float dotProduct;
        float distanceToMaintain;
        float accuracy;

        //Obstacle variables
        Vector3 obstacleAvoidDirection = Vector3.right;
        bool obstacleHit = false;
        float obstacleTimer = 0;
        float avoidTime = 2f;
        float shotTimer = 0.0f;
        float shotInterval = 1.0f;
        float maxSpeed = 0;
        GameObject bulletPrefab;
        GameObject bulletSpawnPos;

        public FighterAttackState(FighterController enemyController, Player playerObj, GameObject bullet, GameObject bulletShootPos, float distanceNeedToMaintain, float accuracyForShot, float fireRate)
        {
            controller = enemyController;
            player = playerObj;
            distanceToMaintain = distanceNeedToMaintain;
            accuracy = accuracyForShot;
            bulletPrefab = bullet;
            bulletSpawnPos = bulletShootPos;
            shotInterval = fireRate;

            stateID = FSMStateID.Attacking;
        }

        public override void Act()
        {
            Move();
            Shoot();
        }

        public override void Reason()
        {
            if (player == null)
            {
                controller.PerformTransition(Transition.Patrol);
            }
            else
            {
                //CalcDotProduct();

                //maxSpeed = Mathf.Max(maxSpeed, controller.rbSelf.velocity.magnitude);

                //float distance = Vector3.Distance(controller.transform.position, player.transform.position);
                //if (dotProduct < 0 && distance < 100)
                //{
                //    controller.PerformTransition(Transition.Patrol); //Go to patrolling
                //}
            }

            //Else dead transition to dead
            if (controller.Health <= 0)
            {
                controller.PerformTransition(Transition.NoHealth);
            }
        }

        public override void EnterStateInit()
        {
            //Debug.Log("Attacking");
        }
        
        //Moves
        void Move()
        {
            if (player != null)
            {
                //Calculate direction
                Vector3 direction = controller.transform.forward; // sets forward
                direction.Normalize();

                AvoidObstacles(ref direction); // will change direction towards the right if an obstacle is in the way

                //rotation
                if (!obstacleHit && obstacleTimer == 0)
                {
                    direction = interceptPoint - controller.transform.position; // sets desired direction to target intercept point

                    //if no obstacles, allow rotation to desired direction
                    Vector3 newDir = Vector3.RotateTowards(controller.transform.forward, direction, controller.RegRotationForce * Time.deltaTime, 0);
                    controller.transform.rotation = Quaternion.LookRotation(newDir);
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

                //Move
                {
                    //maintains a distance
                    float percent = Vector3.Distance(controller.transform.position, player.transform.position) / distanceToMaintain;
                    if (percent > 1) percent = 1;
                    controller.rbSelf.AddForce(controller.transform.forward.normalized * controller.Acceleration * percent, ForceMode.Acceleration);
                }
            }
        }

        void Shoot()
        {
            if (!obstacleHit)
            {
                shotTimer += Time.deltaTime;

                float distanceFromSight = Vector3.Cross(controller.transform.forward, interceptPoint - controller.transform.position).magnitude;

                if (distanceFromSight <= accuracy && shotTimer >= shotInterval)
                {
                    Quaternion lookRot = Quaternion.LookRotation(controller.transform.forward);
                    GameObject bullet = GameObject.Instantiate(bulletPrefab, bulletSpawnPos.transform.position, lookRot);
                    bullet.GetComponent<Bullet>().shooter = controller.gameObject;

                    shotTimer = 0;
                }
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
