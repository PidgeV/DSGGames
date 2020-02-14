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
        // TODO: Warp in effect
        controller.PerformTransition(Transition.Defend);
    }

    public override void Reason()
    {
        //Do nothing
    }
}
