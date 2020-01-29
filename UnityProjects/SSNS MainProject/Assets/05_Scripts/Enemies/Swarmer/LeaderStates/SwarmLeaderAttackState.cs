using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Complete
{
    public class SwarmLeaderAttackState : FSMState
    {
        FlockLeaderController controller;
        Flock swarm;

        public SwarmLeaderAttackState(FlockLeaderController leader, Flock swarmObj)
        {
            controller = leader;
            swarm = swarmObj;

            stateID = FSMStateID.Attacking;
        }

        public override void Act()
        {
            Move();
        }

        public override void Reason()
        {
            if (swarm.defenseTarget != null)
            {
                //Enter defend state mode
                controller.PerformTransition(Transition.Defend);
            }

            if (swarm.player == null)
            {
                swarm.player = GameObject.FindGameObjectWithTag("Player");

                if (swarm.player == null)
                {
                    //If no player can be found then go to patrol state
                    controller.PerformTransition(Transition.Patrol);
                }
            }

            //Determining agent freedom from leader
            if(Vector3.Distance(controller.transform.position, swarm.player.transform.position) <= 100f)
            {
                swarm.swarmFollowRadius = controller.AttackRadius;
            }
            else if(swarm.swarmFollowRadius == controller.AttackRadius)
            {
                swarm.swarmFollowRadius = controller.PatrolRadius;
            }
        }

        public override void EnterStateInit()
        {
            //Do this when entering the state
            //swarm.swarmFollowRadius = controller.AttackRadius;
        }

        void Move()
        {
            if (swarm.player != null)
            {
                //Move towards position. No need to worry about obstacles or 
                controller.transform.position = Vector3.MoveTowards(controller.transform.position, swarm.player.transform.position, controller.AttackSpeed * Time.deltaTime);
            }
        }
    }
}