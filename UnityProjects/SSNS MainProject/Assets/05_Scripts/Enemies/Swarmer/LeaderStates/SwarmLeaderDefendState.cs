using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Complete
{
    /// <summary>
    /// This is a defense state that causes the swarm to move with and surround a given target to the flock script.
    /// This state will override other states and is controlled by a boolean inside Flock.cs. This should be set by the Game Manager based on game state.
    /// </summary>
    public class SwarmLeaderDefendState : FSMState
    {
        FlockLeaderController controller;
        Flock swarm;

        public SwarmLeaderDefendState(FlockLeaderController leaderController, Flock flock)
        {
            controller = leaderController;
            swarm = flock;

            swarm.swarmFollowRadius = controller.DefenseRadius; //Set swarm freedom radius to defense radius

            stateID = FSMStateID.Defend;
        }

        public override void Act()
        {
            Move();
        }

        public override void Reason()
        {
            if(swarm.defenseTarget == null)
            {
                controller.PerformTransition(Transition.Patrol);
            }
        }

        private void Move()
        {
            if(swarm.defenseTarget != null)
            {
                //Moves swarm towards the ship to defend
                controller.transform.position = Vector3.MoveTowards(controller.transform.position, swarm.defenseTarget.transform.position, controller.PatrolSpeed * Time.deltaTime);
            }
        }

        public override void EnterStateInit()
        {
            swarm.swarmFollowRadius = controller.DefenseRadius; //Set swarm freedom radius to defense radius
        }
    }
}