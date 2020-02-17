using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DreadnovaSpawnState : FSMState
{
    private DreadnovaController controller;
    public DreadnovaSpawnState(DreadnovaController enemyController)
    {
        controller = enemyController;
        stateID = FSMStateID.Spawned;
    }

    public override void Act()
    {
        controller.Warp(true);
    }

    public override void Reason()
    {
        controller.PerformTransition(Transition.Defend);
    }
}
