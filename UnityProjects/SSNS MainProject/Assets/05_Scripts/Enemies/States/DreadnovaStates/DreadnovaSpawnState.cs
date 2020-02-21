using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DreadnovaSpawnState : FSMState
{
    private DreadnovaController controller;

    private bool warped;

    public DreadnovaSpawnState(DreadnovaController enemyController)
    {
        controller = enemyController;
        stateID = FSMStateID.Spawned;
    }

    public override void Act()
    {
    }

    public override void Reason()
    {
        if (GameManager.Instance.GameState == SNSSTypes.GameState.BATTLE)
        {
            if (!warped)
            {
                warped = true;
                controller.WarpDreadnova(true);
            }
            else if (!controller.warping)
            {
                controller.PerformTransition(Transition.Defend);
            }
        }
    }
}
