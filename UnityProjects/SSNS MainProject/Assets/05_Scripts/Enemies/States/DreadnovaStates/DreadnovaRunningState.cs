using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DreadnovaEscapeState : FSMState
{
    private DreadnovaController controller;

    private bool warped;

    public DreadnovaEscapeState(DreadnovaController enemyController)
    {
        controller = enemyController;
        stateID = FSMStateID.Running;
    }

    public override void Act()
    {
        // TODO: Warp out effect

        controller.WarpDreadnova(false);

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
                controller.PerformTransition(Transition.NoHealth);

                GameManager.Instance.SwitchState(SNSSTypes.GameState.NODE_SELECTION);
            }
        }
    }
}
