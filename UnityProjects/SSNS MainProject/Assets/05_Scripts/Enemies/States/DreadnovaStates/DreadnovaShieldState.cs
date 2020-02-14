using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DreadnovaShieldState : FSMState
{
    private DreadnovaController controller;

    public DreadnovaShieldState(DreadnovaController enemyController)
    {
        controller = enemyController;
        stateID = FSMStateID.Defend;
    }

    public override void Act()
    {
    }

    public override void Reason()
    {

    }
}
