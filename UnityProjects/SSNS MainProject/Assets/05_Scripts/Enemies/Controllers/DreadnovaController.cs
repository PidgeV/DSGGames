using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DreadnovaController : AdvancedFSM
{
    [SerializeField] private HealthAndShields[] shieldGenerators;

    private DreadnovaSpawner spawner;
    private HealthAndShields health;

    private GameObject player;

    private float waveTime;

    private void ConstructFSM()
    {
        DeadState dead = new DeadState(this);
        DreadnovaSpawnState spawn = new DreadnovaSpawnState(this);
        DreadnovaShieldState shield = new DreadnovaShieldState(this, shieldGenerators);
        DreadnovaEscapeState escape = new DreadnovaEscapeState(this);

        spawn.AddTransition(Transition.Defend, FSMStateID.Defend);

        //shield.AddTransition(Transition.Attack, FSMStateID.Attacking);
        shield.AddTransition(Transition.NoShield, FSMStateID.Running);
        shield.AddTransition(Transition.NoHealth, FSMStateID.Dead);

        escape.AddTransition(Transition.NoHealth, FSMStateID.Dead);

        AddFSMState(spawn);
        AddFSMState(shield);
        AddFSMState(escape);
        AddFSMState(dead);
    }

    protected override void Initialize()
    {
        player = GameObject.FindGameObjectWithTag("Player");

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

    public void Warp(bool warpIn)
    {
        // TODO: Warp effects
        if (warpIn)
        {

        }
        else
        {

        }
    }

    public void DestroyGenerators()
    {
        foreach (HealthAndShields generator in shieldGenerators)
        {
            generator.TakeDamage(Mathf.Infinity, Mathf.Infinity);
        }
    }

    public DreadnovaSpawner Spawner { get { return spawner; } }
    public HealthAndShields Health { get { return health; } }
    public GameObject Player { get { return player; } }
}
