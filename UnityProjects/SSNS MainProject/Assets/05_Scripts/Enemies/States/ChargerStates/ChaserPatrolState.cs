using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargerPatrolState : FSMState
{
    private GameObject player;
    private ChargerController controller;
    private float distance;
    private float playerDist;
    private int patrolID = 0;
    private bool randomPoint;

    //Obstacle variables
    Vector3 obstacleAvoidDirection = Vector3.right;
    bool obstacleHit = false;
    float obstacleTimer = 0;
    float avoidTime = 2f;

    //Timer for staying in patrol;
    private float timer1 = 0f;
    private float timeAfterTransition = 10f;
    //Timer for how often to check for seeing the player
    private float timer2 = 0f;
    private float timeOftenCheck = 1.0f;



    //Constructor
    public ChargerPatrolState(ChargerController enemyController, GameObject playerObj, float waypointDistance, float playerDistance, bool randomizePoint = false)
    {
        controller = enemyController;
        player = playerObj;
        distance = (waypointDistance * 12); // Multiply for meters to units. 12 units/meter
        playerDist = (playerDistance * 12);
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
        //Debug.Log("Patrolling");
        controller.hitPlayer = false;
    }


    public override void Reason()
    {
        if (timer1 < timeAfterTransition)
        {
            timer1 += Time.deltaTime;
        }
        if (timer2 < timeOftenCheck)
        {
            timer2 += Time.deltaTime;
        }

        //Check distance to waypoint
        if (Vector3.Distance(controller.transform.position, controller.waypoints[patrolID].transform.position) < distance)
        {
            if (randomPoint)
            {
                patrolID = Random.Range(0, controller.waypoints.Length);
            }
            else
            {
                patrolID++;

                if (patrolID >= controller.waypoints.Length) patrolID = 0;
            }
        }

        if (player != null)
        {
            float pDist = Vector3.Distance(controller.transform.position, player.gameObject.transform.position);

            //Check distance to player
            if (pDist < playerDist && timer1 >= timeAfterTransition)
            {
                //Debug.DrawLine(controller.transform.position, player.transform.position);

                if (timer2 >= timeOftenCheck)
                {
                    timer2 = 0f;

                    if (PlayerInVision()) // in vision and has been patrolling for minimum time. This is to prevent the ai staying in attack mode and acting weird
                    {
                        timer1 = 0f;
                        controller.PerformTransition(Transition.SawPlayer);
                    }
                }
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
        LayerMask layerMask = LayerMask.GetMask("Obstacles");
        layerMask += LayerMask.GetMask("Player"); //Add player layer to obstacles

        Physics.Raycast(ray, out hitInfo, Mathf.Infinity, layerMask);

        if (hitInfo.collider != null)
        {
            Debug.Log(hitInfo.collider.gameObject.name);
            if (hitInfo.collider.gameObject.Equals(player.gameObject))
            {
                return true;
            }
        }
        return false;
    }

    //Moves
    void Move()
    {
        if (controller.waypoints[patrolID] != null)
        {
            //Calculate direction
            Vector3 direction = controller.transform.forward; // sets forward
            direction.Normalize();

            AvoidObstacles(ref direction); // will change direction towards the right if an obstacle is in the way

            //Rotation
            if (!obstacleHit && obstacleTimer == 0)
            {
                direction = controller.waypoints[patrolID].transform.position - controller.transform.position; // sets desired direction to target intercept point

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
