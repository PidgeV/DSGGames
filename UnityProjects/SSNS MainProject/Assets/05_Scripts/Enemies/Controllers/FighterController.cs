using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FighterController : EnemyController
{
    [SerializeField] float closeDistanceCheck = 90;

    [SerializeField] GameObject bullet;
    [SerializeField] GameObject bulletShootPos;
    //[SerializeField] float distanceNeedToMaintain;
    [SerializeField] float accuracyForShot;

    private void ConstructFSM()
    {
        DeadState deadState = new DeadState(this);
        SpawnState spawnState = new SpawnState(this);
        FighterPatrolState patrol = new FighterPatrolState(this, player, waypointDistanceMeters, playerDistanceMeters, true);
        FighterAttackState attack = new FighterAttackState(this, player, bullet, bulletShootPos);

        spawnState.AddTransition(Transition.Patrol, FSMStateID.Patrolling);

        patrol.AddTransition(Transition.NoHealth, FSMStateID.Dead);
        patrol.AddTransition(Transition.SawPlayer, FSMStateID.Attacking); //Change this

        attack.AddTransition(Transition.NoHealth, FSMStateID.Dead);
        attack.AddTransition(Transition.Patrol, FSMStateID.Patrolling);
        //What's the difference between saw player and attack transition?

        AddFSMState(spawnState);
        AddFSMState(patrol);
        AddFSMState(attack);
        AddFSMState(deadState);
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

        rbSelf = GetComponent<Rigidbody>();
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

    //Getters
    public float CloseDistance { get { return closeDistanceCheck; } }
    public float Health { get { return currentHealth; } }
    public GameObject Player { get { return player; } }
    public float Accuracy { get { return accuracyForShot; } }
    public float FireRate { get { return fireRate; } }
}
