using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Complete
{
    public class EnemyController : AdvancedFSM
    {
        [SerializeField] GameObject destroyedPrefab;
        [SerializeField] GameObject[] waypoints;
        [SerializeField] float waypointDistance = 50;
        [SerializeField] float playerDistance = 100;
        Player player;
        // Start is called before the first frame update
        void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void ConstructFSM()
        {
            DeadState deadState = new DeadState(this, destroyedPrefab);
            PatrolState patrol = new PatrolState(this, player, waypoints, waypointDistance, playerDistance);

            patrol.AddTransition(Transition.NoHealth, FSMStateID.Dead);
            patrol.AddTransition(Transition.SawPlayer, FSMStateID.Chasing);

            AddFSMState(deadState);
        }
    }
}