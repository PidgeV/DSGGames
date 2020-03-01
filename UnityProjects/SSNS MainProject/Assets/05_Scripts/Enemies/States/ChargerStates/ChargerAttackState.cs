﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargerAttackState : AttackState<ChargerController>
{
    private float dotProduct;
    private float currentSpeed;

    //constructor
    public ChargerAttackState(ChargerController chargerController) : base(chargerController)
    {
    }

    public override void Act()
    {
        CalculateIntercept();
        Move();
    }

    public override void Reason()
    {
        if (controller.Player == null || ((ChargerController)controller).hitPlayer)
        {
            controller.PerformTransition(Transition.Patrol);
        }
        else
        {
            CalcDotProduct();

            currentSpeed = Mathf.Max(controller.Stats.attackShipSpeed, controller.Rigid.velocity.magnitude);

            float distance = Vector3.Distance(controller.transform.position, controller.Player.transform.position);
            if (dotProduct < 0 && distance < 100)
            {
                controller.PerformTransition(Transition.Patrol); //Go to patrolling
            }
        }

        //Else dead transition to dead
        if (controller.Health.IsDead)
        {
            controller.PerformTransition(Transition.NoHealth);
        }
    }

    //Calculates the intercept point
    protected override void CalculateIntercept()
    {
        Rigidbody rbTarget = controller.Player.gameObject.GetComponent<Rigidbody>();
        //positions
        Vector3 targetPosition = controller.Player.transform.position;
        //velocities
        Vector3 velocity = controller.Rigid ? controller.Rigid.velocity : Vector3.zero;
        Vector3 targetVelocity = rbTarget ? rbTarget.velocity : Vector3.zero;

        //calculate intercept
        interceptPoint = InterceptCalculationClass.FirstOrderIntercept(controller.transform.position, velocity, currentSpeed, targetPosition, targetVelocity);
    }

    //Moves
    protected override void Move()
    {
        if (interceptPoint != null)
        {
            float rotationForce = controller.Stats.rotationSpeed;
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
                rotationForce = controller.Stats.attackRotationSpeed;
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
                controller.Rigid.AddForce(controller.transform.forward.normalized * currentSpeed, ForceMode.Acceleration); // charge if there's no obstacle
            }
            else
            {
                controller.Rigid.AddForce(controller.transform.forward.normalized * controller.Stats.shipSpeed, ForceMode.Acceleration); // move regular speed if an obstacle is in the way
            }
        }
    }

    protected void CalcDotProduct()
    {
        if (controller.Player != null)
        {
            dotProduct = Vector3.Dot(controller.transform.forward.normalized, (controller.Player.transform.position - controller.transform.position).normalized);
            //Debug.Log(dotProduct);
        }
    }

    protected bool LineOfSight()
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