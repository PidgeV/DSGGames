using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Complete
{
    public class DeadState : FSMState
    {
        private AdvancedFSM controller;
        public DeadState(AdvancedFSM enemyController)
        {
            controller = enemyController;
            stateID = FSMStateID.Dead;
        }

        public override void Act()
        {
            //Do nothing
        }

        public override void Reason()
        {
            //Do nothing
        }
    }
}