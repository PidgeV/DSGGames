using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ChargerController : EnemyController
{
    public bool hitPlayer = false;

    protected override void ConstructFSM()
    {
        DeadState deadState = new DeadState(this);
        SpawnState spawnState = new SpawnState(this);
        ChargerPatrolState patrol = new ChargerPatrolState(this, player, waypointDistanceMeters, playerDistanceMeters, true);
        ChargerAttackState attack = new ChargerAttackState(this, player);

        spawnState.AddTransition(Transition.Patrol, FSMStateID.Patrolling);

        patrol.AddTransition(Transition.NoHealth, FSMStateID.Dead);
        patrol.AddTransition(Transition.SawPlayer, FSMStateID.Attacking); //Change this

        attack.AddTransition(Transition.NoHealth, FSMStateID.Dead);
        attack.AddTransition(Transition.Patrol, FSMStateID.Patrolling);
        //What's the difference between saw player and attack transition?

        AddFSMState(spawnState);
        AddFSMState(patrol);
        AddFSMState(deadState);
        AddFSMState(attack);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (player && collision.gameObject.Equals(player) && CurrentStateID == FSMStateID.Attacking)
        {
            hitPlayer = true;
            PerformTransition(Transition.Patrol);
        }
    }

    //Draw debug rays
    private void OnDrawGizmos()
    {
        if (debugDraw)
        {
            Debug.DrawRay(transform.position, transform.forward.normalized * collisionCheckDistance, Color.green); //Forward ray
                                                                                                                   //Debug.DrawRay(transform.position, interceptPoint - transform.position, Color.red); // Intercept point
            if (rbSelf)
            {
                //Vector3 dir = rbSelf.velocity;
                Debug.DrawRay(transform.position, rbSelf.velocity.normalized * collisionCheckDistance, Color.blue); //Velocity ray
            }
        }
    }

    public bool PlayerInVision()
    {
        Vector3 dir = player.transform.position - transform.position;
        Ray ray = new Ray(transform.position, dir);
        RaycastHit hitInfo;
        LayerMask layerMask = LayerMask.GetMask("Obstacles");
        layerMask += LayerMask.GetMask("Player"); //Add player layer to obstacles
        layerMask += LayerMask.GetMask("Enemies");

        Physics.Raycast(ray, out hitInfo, Mathf.Infinity, layerMask);

        if (hitInfo.collider != null)
        {
            //Debug.Log(hitInfo.collider.gameObject.name);
            if (hitInfo.collider.gameObject.Equals(player.gameObject))
            {
                return true;
            }
        }
        return false;
    }

    public bool AvoidObstacles(ref Vector3 dir)
    {
        bool hit = false;
        RaycastHit hitInfo;

        //Check direction facing
        if (Physics.SphereCast(transform.position, RaySize, transform.forward.normalized, out hitInfo, CollisionCheckDistance, ObstacleLayer))/* || Physics.SphereCast(controller.transform.position, controller.RaySize, controller.rbSelf.velocity.normalized, out hitInfo, controller.CollisionCheckDistance, controller.ObstacleLayer))*/
        {
            // Get the desired direction we need to move to move around  the obstacle. Transform to world co-ordinates (gets the obstacleMoveDirection wrt the current foward direction).
            Vector3 turnDir = transform.TransformDirection(hitInfo.normal);
            turnDir.Normalize();

            dir += turnDir;
            hit = true;
        }
        if (Physics.SphereCast(transform.position, RaySize, rbSelf.velocity.normalized, out hitInfo, CollisionCheckDistance, ObstacleLayer))
        {
            // Get the desired direction we need to move to move around  the obstacle. Transform to world co-ordinates (gets the obstacleMoveDirection wrt the current foward direction).
            Vector3 turnDir = transform.TransformDirection(hitInfo.normal);
            turnDir.Normalize();

            dir += turnDir;
            hit = true;
        }
        return hit;
    }

    //Getters
    public float PlayerDistance { get { return playerDistanceMeters; } }
    public float WaypointDistance { get { return waypointDistanceMeters; } }
    public float CollisionCheckDistance { get { return collisionCheckDistance; } }
    public LayerMask ObstacleLayer { get { return obstacleLayer; } }
    public float RaySize { get { return raySize; } }
    public float RegRotationForce { get { return stats.regRotationForce; } }
    public float ChargeRotationForce { get { return stats.chargeRotationForce; } }
    public float Acceleration { get { return stats.acceleration; } }
    public float ChargeAcceleration { get { return stats.chargeAcceleration; } }
    public float Health { get { return stats.currentHealth; } }
    public GameObject Player { get { return player; } }
}