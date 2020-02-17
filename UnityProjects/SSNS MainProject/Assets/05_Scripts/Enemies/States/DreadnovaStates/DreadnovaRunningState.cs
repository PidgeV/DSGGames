using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DreadnovaEscapeState : FSMState
{
    private DreadnovaController controller;
    public DreadnovaEscapeState(DreadnovaController enemyController)
    {
        controller = enemyController;
        stateID = FSMStateID.Running;
    }

    public override void Act()
    {
        // TODO: Warp out effect

        controller.Warp(false);

    }

    public override void Reason()
    {
        controller.PerformTransition(Transition.NoHealth);
    }
}
