using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ChargerController : AdvancedFSM
{
    public GameObject[] waypoints;

    [Header("Green: forward direction and collision check.")]
    [Header("Red: intercept calculation.")]
    [Header("Blue: Velocity and collision check.")]
    [SerializeField] bool debugDraw = false;

    [SerializeField] ChargerStats stats;

    //MAth things for later. To store the dot product
    float dotProduct;

    [Space(15)]
    [SerializeField] LayerMask obstacleLayer;
    [SerializeField] float collisionCheckDistance = 150f;

    [Space(15)]
    [SerializeField] float waypointDistanceMeters = 50f;
    [SerializeField] float playerDistanceMeters = 100f;

    public Rigidbody rbSelf;

    [Tooltip("Size of ray for collision checking. Larger numbers will mean the avoidance is larger")]
    [SerializeField] float raySize = 7.5f;

    GameObject player;
    public bool hitPlayer = false;

    private void ConstructFSM()
    {
        DeadState deadState = new DeadState(this);
        ChargerPatrolState patrol = new ChargerPatrolState(this, player, waypoints, waypointDistanceMeters, playerDistanceMeters, true);
        ChargerAttackState attack = new ChargerAttackState(this, player);

        patrol.AddTransition(Transition.NoHealth, FSMStateID.Dead);
        patrol.AddTransition(Transition.SawPlayer, FSMStateID.Attacking); //Change this

        attack.AddTransition(Transition.NoHealth, FSMStateID.Dead);
        attack.AddTransition(Transition.Patrol, FSMStateID.Patrolling);
        //What's the difference between saw player and attack transition?

        AddFSMState(patrol);
        AddFSMState(deadState);
        AddFSMState(attack);
    }

    protected override void Initialize()
    {
        //Pretty self explainatory
        player = GameObject.FindGameObjectWithTag("Player");
        stats.currentHealth = stats.maxHealth;
        ConstructFSM();

    }
    protected override void FSMUpdate()
    {
        //Do this
        if (CurrentState != null)
        {
            CurrentState.Reason();
            CurrentState.Act();
        }
    }
    protected override void FSMFixedUpdate()
    {
        //Guess we needed this
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (player && collision.gameObject.Equals(player) && CurrentStateID == FSMStateID.Attacking)
        {
            hitPlayer = true;
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
            Debug.Log(hitInfo.collider.gameObject.name);
            if (hitInfo.collider.gameObject.Equals(player.gameObject))
            {
                return true;
            }
        }
        return false;
    }

    public bool AvoidObstacles(ref Vector3 dir)
    {
        RaycastHit hitInfo;

        //Check direction facing
        if (Physics.SphereCast(transform.position, RaySize, transform.forward.normalized,
            out hitInfo, CollisionCheckDistance, ObstacleLayer) /*||
            Physics.SphereCast(controller.transform.position, controller.RaySize, controller.rbSelf.velocity.normalized,
            out hitInfo, controller.CollisionCheckDistance, controller.ObstacleLayer)*/)
        {
            // Get the desired direction we need to move to move around  the obstacle. Transform to world co-ordinates (gets the obstacleMoveDirection wrt the current foward direction).
            Vector3 turnDir = transform.TransformDirection(hitInfo.normal * 2);
            turnDir.Normalize();

            dir += turnDir;
            return true;
        }
        return false;
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