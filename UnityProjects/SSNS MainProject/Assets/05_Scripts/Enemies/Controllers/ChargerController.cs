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
        ChargerPatrolState patrol = new ChargerPatrolState(this, true);
        ChargerAttackState attack = new ChargerAttackState(this);

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
        if (Player && collision.gameObject.Equals(Player) && CurrentStateID == FSMStateID.Attacking)
        {
            hitPlayer = true;
            PerformTransition(Transition.Patrol);
        }
    }
}