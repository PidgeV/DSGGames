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
    }

    public override void Reason()
    {
        if (shieldAliveCount <= 0)
        {
            if (NodeManager.Instance.CurrentNode.Type == SNSSTypes.NodeType.MiniBoss)
                controller.PerformTransition(Transition.NoShield);
            else
                controller.PerformTransition(Transition.Attack);
        }
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
