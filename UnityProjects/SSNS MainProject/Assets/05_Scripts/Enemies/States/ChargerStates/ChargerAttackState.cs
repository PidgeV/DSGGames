using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargerAttackState : FSMState
{
    ChargerController controller;
    private GameObject player;
    Vector3 interceptPoint;
    float dotProduct;

    //Obstacle variables
    Vector3 obstacleAvoidDirection = Vector3.right;
    bool obstacleHit = false;
    float obstacleTimer = 0;
    float avoidTime = 2f;

    float maxSpeed = 0;

    //constructor
    public ChargerAttackState(ChargerController chaserController, GameObject playerObj)
    {
        controller = chaserController;
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
        if (player == null || controller.hitPlayer)
        {
            AIManager.aiManager.StopAttack();
            controller.PerformTransition(Transition.Patrol);
        }
        else
        {

            CalcDotProduct();

            maxSpeed = Mathf.Max(maxSpeed, controller.rbSelf.velocity.magnitude);

            float distance = Vector3.Distance(controller.transform.position, player.transform.position);
            if (dotProduct < 0 && distance < 100)
            {
                AIManager.aiManager.StopAttack();
                controller.PerformTransition(Transition.Patrol); //Go to patrolling
            }
        }

        //Else dead transition to dead
        if (controller.Health <= 0)
        {
            AIManager.aiManager.StopAttack();
            controller.PerformTransition(Transition.NoHealth);
        }
    }

    public override void EnterStateInit()
    {
        //Debug.Log("Attacking");
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
        interceptPoint = InterceptCalculationClass.FirstOrderIntercept(controller.transform.position, velocity, maxSpeed, targetPosition, targetVelocity);
    }

    //Moves
    void Move()
    {
        if (interceptPoint != null)
        {
            float rotationForce = controller.RegRotationForce;
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
                direction = interceptPoint - controller.transform.position; // sets desired direction to target intercept point
                rotationForce = controller.ChargeRotationForce;
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

            Vector3 newDir = Vector3.RotateTowards(controller.transform.forward, direction, rotationForce * Time.deltaTime, 0);
            Quaternion rot = Quaternion.LookRotation(newDir);
            controller.transform.rotation = Quaternion.Slerp(controller.transform.rotation, rot, rotationForce * Time.deltaTime);

            //Movement
            if (!obstacleHit && LineOfSight() && dotProduct > 0.95f)
            {
                controller.rbSelf.AddForce(controller.transform.forward.normalized * controller.ChargeAcceleration, ForceMode.Acceleration); // charge if there's no obstacle
            }
            else
            {
                controller.rbSelf.AddForce(controller.transform.forward.normalized * controller.Acceleration, ForceMode.Acceleration); // move regular speed if an obstacle is in the way
            }
        }
    }

    void CalcDotProduct()
    {
        if (player != null)
        {
            dotProduct = Vector3.Dot(controller.transform.forward.normalized, (player.transform.position - controller.transform.position).normalized);
            //Debug.Log(dotProduct);
        }
    }

    bool LineOfSight()
    {
        //Raycast to see if there is a straight shot to the intercept point
        Ray ray = new Ray(controller.transform.position, interceptPoint);
        RaycastHit[] hitInfo = Physics.RaycastAll(ray);

        if (hitInfo.Length <= 1 && !obstacleHit && dotProduct > 0.8f) //Because I dont want to exclude itself from collision detection
        {
            return true;
        }

        return false;
    }
}