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
        [SerializeField] float defenseStateRadius;

        [Space(15)]
        [SerializeField] float waypointDistanceMeters = 75f;
        [SerializeField] float playerDistanceMeters = 200f;

        public float WaypointDistance { get { return waypointDistanceMeters; } }
        public float PlayerDistance { get { return playerDistanceMeters; } }
        public float PatrolSpeed { get { return patrolSpeed; } }
        public float AttackSpeed { get { return attackSpeed; } }
        public float AttackRadius { get { return attackStateRadius; } }
        public float PatrolRadius { get { return patrolStateRadius; } }
        public float DefenseRadius { get { return defenseStateRadius; } }

        protected override void FSMFixedUpdate()
        {
            
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
            SwarmLeaderDefendState defend = new SwarmLeaderDefendState(this, swarm);

            //Transitions
            patrol.AddTransition(Transition.Attack, FSMStateID.Attacking);
            patrol.AddTransition(Transition.Patrol, FSMStateID.Patrolling);
            
            attack.AddTransition(Transition.Patrol, FSMStateID.Patrolling);
            attack.AddTransition(Transition.Defend, FSMStateID.Defend);

            defend.AddTransition(Transition.Patrol, FSMStateID.Patrolling);

            //Add states
            AddFSMState(defend);
            AddFSMState(patrol);
            AddFSMState(attack);
        }
    }
}