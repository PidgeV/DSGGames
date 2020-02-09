﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FighterController : AdvancedFSM
{
    public GameObject[] waypoints;

    [Header("Green: forward direction and collision check.")]
    [Header("Red: intercept calculation.")]
    [Header("Blue: Velocity and collision check.")]
    [SerializeField] bool debugDraw = false;

    [SerializeField] float fireRate;//FighterStats stats;
    [SerializeField] float maxHealth;
    [SerializeField] float currentHealth;

    [SerializeField] float regRotationForce;
    [SerializeField] float acceleration;

    //MAth things for later. To store the dot product
    float dotProduct;

    [Space(15)]
    [SerializeField] LayerMask obstacleLayer;
    [SerializeField] float collisionCheckDistance = 150f;

    [Space(15)]
    [SerializeField] float waypointDistanceMeters = 50f;
    [SerializeField] float playerDistanceMeters = 100f;
    [SerializeField] float followDistance = 100;
    [SerializeField] float closeDistance = 90;

    public Rigidbody rbSelf;

    [Tooltip("Size of ray for collision checking. Larger numbers will mean the avoidance is larger")]
    [SerializeField] float raySize = 7.5f;

    GameObject player;
    [SerializeField] GameObject bullet;
    [SerializeField] GameObject bulletShootPos;
    //[SerializeField] float distanceNeedToMaintain;
    [SerializeField] float accuracyForShot;

    private void ConstructFSM()
    {
        DeadState deadState = new DeadState(this);
        FighterPatrolState patrol = new FighterPatrolState(this, player, waypoints, waypointDistanceMeters, playerDistanceMeters, true);
        FighterAttackState attack = new FighterAttackState(this, player, bullet, bulletShootPos);

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
        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        currentHealth = maxHealth;
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

    //Getters
    public float PlayerDistance { get { return playerDistanceMeters; } }
    public float WaypointDistance { get { return waypointDistanceMeters; } }
    public float FollowDistance { get { return followDistance; } }
    public float CloseDistance { get { return closeDistance; } }
    public float CollisionCheckDistance { get { return collisionCheckDistance; } }
    public LayerMask ObstacleLayer { get { return obstacleLayer; } }
    public float RaySize { get { return raySize; } }
    public float RegRotationForce { get { return regRotationForce; } }
    public float Acceleration { get { return acceleration; } }
    public float Health { get { return currentHealth; } }
    public GameObject Player { get { return player; } }
    public float Accuracy { get { return accuracyForShot; } }
    public float FireRate { get { return fireRate; } }
}
