using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DreadnovaController : AdvancedFSM
{
    [SerializeField] private HealthAndShields[] shieldGenerators;
    public GameObject dreadnovaShield;
    public GameObject dreadnovaModel;
    public GameObject dreadnovaThrusters;

    private DreadnovaSpawner spawner;
    private HealthAndShields health;

    private GameObject player;

    private float waveTime;

    public bool warping;

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

        transform.position += transform.forward * -100000;
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

    public void WarpDreadnova(bool warpIn)
    {
        StartCoroutine(Warp(warpIn));
    }

    public void DestroyGenerators()
    {
        foreach (HealthAndShields generator in shieldGenerators)
        {
            generator.TakeDamage(Mathf.Infinity, Mathf.Infinity);
        }
    }

    private IEnumerator Warp(bool warpIn)
    {
        warping = true;

        // TODO: Warp effects
        if (warpIn)
        {
            dreadnovaThrusters.SetActive(false);

            yield return new WaitForSeconds(1.5f);

            dreadnovaThrusters.SetActive(true);
            transform.position = AreaManager.Instance.AreaLocation;

            yield return new WaitForSeconds(1.0f);

            dreadnovaThrusters.SetActive(false);
        }
        else
        {
            dreadnovaThrusters.SetActive(true);

            yield return new WaitForSeconds(1.5f);

            transform.position += transform.forward * 1000;

            yield return new WaitForSeconds(1.5f);

            yield return new WaitForSeconds(5.0f);

            dreadnovaThrusters.SetActive(false);
            dreadnovaModel.SetActive(false);
        }

        warping = false;
    }

    public DreadnovaSpawner Spawner { get { return spawner; } }
    public HealthAndShields Health { get { return health; } }
    public GameObject Player { get { return player; } }
}
