using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DreadnovaShieldState : FSMState
{
    private DreadnovaController controller;

    private HealthAndShields[] generators;
    private int shieldAliveCount;

    public DreadnovaShieldState(DreadnovaController enemyController, HealthAndShields[] shieldGenerators)
    {
        controller = enemyController;
        stateID = FSMStateID.Defend;
        generators = shieldGenerators;

        for (int i = 0; i < shieldGenerators.Length; i++)
        {
            shieldGenerators[i].onDeath += OnGeneratorDeath;

            shieldAliveCount++;
        }
    }

    public override void Act()
    {
        if (!controller.GeneratorPhaseOnly && shieldAliveCount <= 0)
        {
            controller.PerformTransition(Transition.Attack);
        }
    }

    public override void Reason()
    {

    }

    public void OnGeneratorDeath()
    {
        shieldAliveCount--;
    }

    ~DreadnovaShieldState()
    {
        shieldAliveCount = 0;

        for (int i = 0; i < generators.Length; i++)
        {
            generators[i].onDeath -= OnGeneratorDeath;
        }

        generators = null;
    }
}
