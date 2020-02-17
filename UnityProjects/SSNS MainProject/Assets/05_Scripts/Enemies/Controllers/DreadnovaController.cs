using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DreadnovaController : AdvancedFSM
{
    [SerializeField] private HealthAndShields[] shieldGenerators;
    [SerializeField] private bool generatorPhaseOnly = true;

    private DreadnovaSpawner spawner;
    private HealthAndShields health;

    private GameObject player;

    private float waveTime;

    private void ConstructFSM()
    {
        DeadState dead = new DeadState(this);
        DreadnovaSpawnState spawn = new DreadnovaSpawnState(this);
        DreadnovaShieldState shield = new DreadnovaShieldState(this, shieldGenerators);

        spawn.AddTransition(Transition.Defend, FSMStateID.Defend);

        //shield.AddTransition(Transition.NoShield, FSMStateID.Attacking);
        //shield.AddTransition(Transition.)

        AddFSMState(spawn);
        AddFSMState(shield);
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

    public bool GeneratorPhaseOnly { get { return generatorPhaseOnly; } }
    public DreadnovaSpawner Spawner { get { return spawner; } }
    public HealthAndShields Health { get { return health; } }
    public GameObject Player { get { return player; } }
}
