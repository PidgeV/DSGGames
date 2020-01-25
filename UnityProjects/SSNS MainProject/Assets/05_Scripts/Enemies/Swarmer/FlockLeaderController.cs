using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Complete
{
    public class FlockLeaderController : AdvancedFSM
    {
        public GameObject[] waypoints;

        [SerializeField] float speed;

        [SerializeField] float attackStateRadius;
        [SerializeField] float patrolStateRadius;

        [Space(15)]
        [SerializeField] float waypointDistanceMeters = 75f;
        [SerializeField] float playerDistanceMeters = 200f;

        public float WaypointDistance { get { return waypointDistanceMeters; } }
        public float PlayerDistance { get { return playerDistanceMeters; } }
        public float Speed { get { return speed; } }
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
            //States
            SwarmLeaderPatrolState patrol = new SwarmLeaderPatrolState(this, transform.parent.GetComponent<Flock>(), waypoints);

            //Transitions
            //patrol.AddTransition(Transition.SawPlayer, FSMStateID.Attacking);

            //Add states
            AddFSMState(patrol);
        }
    }
}