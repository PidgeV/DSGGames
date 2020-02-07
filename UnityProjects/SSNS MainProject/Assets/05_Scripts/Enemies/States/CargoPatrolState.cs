using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CargoPatrolState : FSMState
{
    private GameObject player;
    private CargoController controller;
    private GameObject capitalship;
    private float distance;

    //Obstacle variables
    Vector3 obstacleAvoidDirection = Vector3.right;
    bool obstacleHit = false;
    float obstacleTimer = 0;
    float avoidTime = 2f;
    bool reachedShip = false;

    //Constructor
    public CargoPatrolState(CargoController enemyController, GameObject playerObj, GameObject capitalShip)
    {
        controller = enemyController;
        player = playerObj;
        capitalship = capitalShip;
        distance = (enemyController.CapitalShipDistance * 12); // Multiply for meters to units. 12 units/meter

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
        //Debug.Log("Patrolling");
        controller.hitPlayer = false;
    }


    public override void Reason()
    {
        //Check distance to the capital ship
        if (Vector3.Distance(controller.transform.position, capitalship.transform.position) < distance && !reachedShip)
        {
            //Do reaching capitol ship things
            reachedShip = true;
            Debug.Log("reached the ship");


        }

        //If dead transition to dead
        if (controller.Health <= 0)
        {
            controller.PerformTransition(Transition.NoHealth);
        }
    }

    //Moves
    void Move()
    {
        if (capitalship != null && !reachedShip)
        {
            //Calculate direction
            Vector3 direction = controller.transform.forward; // sets forward
            direction.Normalize();

            AvoidObstacles(ref direction); // will change direction towards the right if an obstacle is in the way

            //Rotation
            if (!obstacleHit && obstacleTimer == 0)
            {
                direction = capitalship.transform.position - controller.transform.position; // sets desired direction to target intercept point

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
            out hitInfo, controller.CollisionCheckDistance, controller.ObstacleLayer) ||
            Physics.SphereCast(controller.transform.position, controller.RaySize, controller.rbSelf.velocity.normalized,
            out hitInfo, controller.CollisionCheckDistance, controller.ObstacleLayer))
        {
            // Get the desired direction we need to move to move around  the obstacle. Transform to world co-ordinates (gets the obstacleMoveDirection wrt the current foward direction).
            Vector3 turnDir = controller.transform.TransformDirection(hitInfo.normal + Vector3.right);
            turnDir.Normalize();

            dir += turnDir;
            obstacleHit = true;
        }
    }
}