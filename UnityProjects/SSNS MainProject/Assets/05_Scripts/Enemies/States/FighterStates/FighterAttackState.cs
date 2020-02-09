using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterAttackState : FSMState
{
    private GameObject player;
    private FighterController controller;
    Vector3 interceptPoint;

    //Obstacle variables
    Vector3 obstacleAvoidDirection = Vector3.right;
    bool obstacleHit = false;
    float obstacleTimer = 0;
    float avoidTime = 0.7f;
    float shotTimer = 0.0f;
    float intervalTime = 0.0f;
    float calculateInterval = 0.1f;
    float maxSpeed = 0;
    GameObject bulletPrefab;
    GameObject bulletSpawnPos;
    Rigidbody rbPlayer;

    public FighterAttackState(FighterController enemyController, GameObject playerObj, GameObject bullet, GameObject bulletShootPos)
    {
        controller = enemyController;
        player = playerObj;
        bulletPrefab = bullet;
        bulletSpawnPos = bulletShootPos;

        stateID = FSMStateID.Attacking;

        rbPlayer = player.GetComponent<Rigidbody>();
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
            player = GameObject.FindGameObjectWithTag("Player");

            if (player == null)
            {
                controller.PerformTransition(Transition.Patrol);
                return;
            }
        }

        CalculateIntercept();

        if (Vector3.Distance(player.transform.position, controller.transform.position) < controller.CloseDistance)
        {
            controller.PerformTransition(Transition.Patrol);
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
            }

            Vector3 newDir = Vector3.RotateTowards(controller.transform.forward, direction, controller.RegRotationForce * Time.deltaTime, 0);
            Quaternion rot = Quaternion.LookRotation(newDir);
            controller.transform.rotation = Quaternion.Slerp(controller.transform.rotation, rot, Time.deltaTime);

            //Move
            //maintains a distance
            //float percent = Vector3.Distance(controller.transform.position, player.transform.position) / 200;
            //if (percent > 1) percent = 1;
            controller.rbSelf.AddForce(controller.transform.forward.normalized * controller.Acceleration /** percent*/, ForceMode.Acceleration);
        }
    }

    void Shoot()
    {
        if (!obstacleHit)
        {
            shotTimer += Time.deltaTime;

            float distanceFromSight = Vector3.Cross(controller.transform.forward, interceptPoint - controller.transform.position).magnitude;

            if (distanceFromSight <= controller.Accuracy && shotTimer >= controller.FireRate)
            {
                Quaternion lookRot = Quaternion.LookRotation(controller.transform.forward);
                GameObject bullet = Object.Instantiate(bulletPrefab, bulletSpawnPos.transform.position, lookRot);
                bullet.GetComponent<Bullet>().shooter = controller.gameObject;

                shotTimer = 0;
            }
        }
    }

    void AvoidObstacles(ref Vector3 dir)
    {
        RaycastHit hitInfo;

        //Check direction facing
        if (Physics.SphereCast(controller.transform.position, controller.RaySize, controller.transform.forward.normalized, out hitInfo, controller.CollisionCheckDistance, controller.ObstacleLayer))/* || Physics.SphereCast(controller.transform.position, controller.RaySize, controller.rbSelf.velocity.normalized, out hitInfo, controller.CollisionCheckDistance, controller.ObstacleLayer))*/
        {
            // Get the desired direction we need to move to move around  the obstacle. Transform to world co-ordinates (gets the obstacleMoveDirection wrt the current foward direction).
            Vector3 turnDir = controller.transform.TransformDirection(hitInfo.normal);
            turnDir.Normalize();

            dir += turnDir;
            obstacleHit = true;
        }
    }

    //Calculates the intercept point
    void CalculateIntercept()
    {
        intervalTime += Time.deltaTime;
        //while (true)
        if (intervalTime >= calculateInterval)
        {
            intervalTime = 0.0f;
            //yield return new WaitForSeconds(calculateInterval);
            //positions
            Vector3 targetPosition = player.transform.position;
            //velocities
            //Vector3 velocity = rbSelf ? rbSelf.velocity : Vector3.zero;
            Vector3 velocity = Vector3.zero;
            Vector3 targetVelocity = rbPlayer ? rbPlayer.velocity : Vector3.zero;

            //calculate intercept
            interceptPoint = InterceptCalculationClassNoMono.FirstOrderIntercept(controller.transform.position, velocity, bulletPrefab.GetComponent<ShotInfo>().Speed, targetPosition, targetVelocity);
        }
    }
}
