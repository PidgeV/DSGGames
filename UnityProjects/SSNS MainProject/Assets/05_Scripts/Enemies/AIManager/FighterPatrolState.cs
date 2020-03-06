using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterPatrolState : FSMState
{
    private GameObject player;
    private FighterController controller;
    private float distance;
    private float playerDist;
    private int patrolID = 0;
    private bool randomPoint;

    //Obstacle variables
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
    public FighterPatrolState(FighterController enemyController, GameObject playerObj, float waypointDistance, float playerDistance, bool randomizePoint = false)
    {
        controller = enemyController;
        player = playerObj;
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
        //Debug.Log("Patrolling");
        timer1 = 0f;
        timer2 = 0f;
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
                if (timer2 >= timeOftenCheck)
                {
                    timer2 = 0f;
                    if (controller.PlayerInVision() && AIManager.aiManager.CanAttack()) // in vision and has been patrolling for minimum time. This is to prevent the ai staying in attack mode and acting weird
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

    //Moves
    void Move()
    {
        if (controller.waypoints[patrolID] != null)
        {
            //Calculate direction
            Vector3 direction = controller.transform.forward; // sets forward
            direction.Normalize();

            if (controller.AvoidObstacles(ref direction)) // will change direction towards the right if an obstacle is in the way
            {
                obstacleHit = true;
            }

            //Rotation
            if (!obstacleHit && obstacleTimer == 0)
            {
                direction = controller.waypoints[patrolID].transform.position - controller.transform.position; // sets desired direction to target intercept point
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
            controller.transform.rotation = Quaternion.Lerp(controller.transform.rotation, rot, controller.RegRotationForce * Time.deltaTime);

            //Movement
            controller.rbSelf.AddForce(controller.transform.forward.normalized * controller.Acceleration, ForceMode.Acceleration); // move regular speed if obstacle is in the way or player is not target
        }
    }
}