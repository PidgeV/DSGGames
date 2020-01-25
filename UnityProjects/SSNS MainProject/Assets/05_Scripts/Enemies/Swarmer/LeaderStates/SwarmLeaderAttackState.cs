using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Complete
{
    public class SwarmLeaderAttackState : FSMState
    {
        FlockLeaderController controller;
        Flock swarm;
        GameObject player;

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
            if (player == null)
            {
                player = GameObject.FindGameObjectWithTag("Player");

                if (player == null)
                {
                    //If no player can be found then go to patrol state
                    controller.PerformTransition(Transition.Patrol);
                }
            }

            if(Vector3.Distance(controller.transform.position, player.transform.position) <= 100f)
            {
                swarm.swarmFollowRadius = controller.AttackRadius;
            }
        }

        public override void EnterStateInit()
        {
            //Do this when entering the state
            //swarm.swarmFollowRadius = controller.AttackRadius;
        }

        void Move()
        {
            if (player != null)
            {
                //Move towards position. No need to worry about obstacles or 
                controller.transform.position = Vector3.MoveTowards(controller.transform.position, player.transform.position, controller.AttackSpeed * Time.deltaTime);
            }
        }
    }
}