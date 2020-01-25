using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Complete
{
    public class FlockLeaderController : AdvancedFSM
    {
        public GameObject[] waypoints;

        [SerializeField] float patrolSpeed;
        [SerializeField] float attackSpeed;

        [SerializeField] float attackStateRadius;
        [SerializeField] float patrolStateRadius;

        [Space(15)]
        [SerializeField] float waypointDistanceMeters = 75f;
        [SerializeField] float playerDistanceMeters = 200f;

        public float WaypointDistance { get { return waypointDistanceMeters; } }
        public float PlayerDistance { get { return playerDistanceMeters; } }
        public float PatrolSpeed { get { return patrolSpeed; } }
        public float AttackSpeed { get { return attackSpeed; } }
        public float AttackRadius { get { return attackStateRadius; } }
        public float PatrolRadius { get { return patrolStateRadius; } }

        protected override void FSMFixedUpdate()
        {
            base.FSMFixedUpdate();
        }

        protected override void FSMUpdate()
        {
            //Do this
            if (CurrentState != null)
            {
                CurrentState.Reason();
                CurrentState.Act();
            }
        }

        protected override void Initialize()
        {
            ConstructFSM();
        }

        void ConstructFSM()
        {
            Flock swarm = transform.parent.GetComponent<Flock>();
            //States
            SwarmLeaderPatrolState patrol = new SwarmLeaderPatrolState(this, swarm, waypoints);
            SwarmLeaderAttackState attack = new SwarmLeaderAttackState(this, swarm);

            //Transitions
            patrol.AddTransition(Transition.Attack, FSMStateID.Attacking);
            patrol.AddTransition(Transition.Patrol, FSMStateID.Patrolling);

            //Add states
            AddFSMState(patrol);
            AddFSMState(attack);
        }
    }
}