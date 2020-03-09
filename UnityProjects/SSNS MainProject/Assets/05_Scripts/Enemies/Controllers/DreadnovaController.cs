﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SNSSTypes;

public class DreadnovaController : EnemyController
{
    [SerializeField] private ShieldGenerator[] shieldGenerators;
    public GameObject dreadnovaShield;
    public GameObject dreadnovaModel;
    public GameObject dreadnovaThrusters;

    [SerializeField] private DreadnovaState dreadnovaState;

    private DreadnovaSpawner spawner;

    public bool warping;

    public override void ResetEnemy()
    {
        base.ResetEnemy();

        foreach (ShieldGenerator generator in shieldGenerators)
        {
            generator.InitializeGenerator();
        }

        dreadnovaShield.SetActive(true);
        dreadnovaModel.SetActive(true);
        dreadnovaThrusters.SetActive(true);

        warping = false;

        spawner.enabled = true;
    }

    protected override void Initialize()
    {
        Spawn = transform.position;
        SpawnDestination = transform.position + transform.forward;

        dreadnovaThrusters.SetActive(true);

        TryGetComponent(out spawner);

        base.Initialize();
    }

    protected override void ConstructFSM()
    {
        DeadState dead = new DeadState(this);
        DreadnovaSpawnState spawn = new DreadnovaSpawnState(this);
        DreadnovaShieldState shield = new DreadnovaShieldState(this);
        DreadnovaEscapeState escape = new DreadnovaEscapeState(this);

        dead.AddTransition(Transition.Reset, FSMStateID.Spawned);

        spawn.AddTransition(Transition.Defend, FSMStateID.Defend);
        spawn.AddTransition(Transition.Reset, FSMStateID.Spawned);

        //shield.AddTransition(Transition.Attack, FSMStateID.Attacking);
        shield.AddTransition(Transition.NoShield, FSMStateID.Running);
        shield.AddTransition(Transition.NoHealth, FSMStateID.Dead);
        shield.AddTransition(Transition.Reset, FSMStateID.Spawned);

        escape.AddTransition(Transition.NoHealth, FSMStateID.Dead);
        escape.AddTransition(Transition.Reset, FSMStateID.Spawned);

        AddFSMState(spawn);
        AddFSMState(shield);
        AddFSMState(escape);
        AddFSMState(dead);
    }

    public void WarpDreadnova()
    {
        StartCoroutine(WarpOut());
    }

    public void DestroyGenerators()
    {
        // TODO: Need to update
        //foreach (HealthAndShields generator in shieldGenerators)
        //{
        //    generator.TakeDamage(Mathf.Infinity, Mathf.Infinity);
        //}
    }

    private IEnumerator WarpOut()
    {
        warping = true;

        // TODO: Warp effects
        dreadnovaThrusters.SetActive(true);

        yield return new WaitForSeconds(1.5f);

        transform.position = Vector3.zero;

        yield return new WaitForSeconds(1.5f);

        yield return new WaitForSeconds(5.0f);

        warping = false;
    }

    public DreadnovaSpawner Spawner { get { return spawner; } }
    public DreadnovaState State { get { return dreadnovaState; } }
    public ShieldGenerator[] Generators { get { return shieldGenerators; } }
}
