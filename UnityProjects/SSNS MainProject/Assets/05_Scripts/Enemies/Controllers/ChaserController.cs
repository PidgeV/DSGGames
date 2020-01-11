using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Complete
{
    public class ChaserController : AdvancedFSM
    {
        [SerializeField] GameObject destroyedPrefab;
        [SerializeField] GameObject[] waypoints;
      
        [Header("Green: forward direction and collision check.")]
        [Header("Red: intercept calculation.")]
        [Header("Blue: Velocity and collision check.")]
        [SerializeField] bool debugDraw = false;

        [SerializeField] float dot;

        [Space(15)]
        [SerializeField] LayerMask obstacleLayer;
        [SerializeField] float collisionCheckDistance = 150f;

        [Space(15)]
        [Tooltip("The acceleration of the ship in meters per second")]
        [SerializeField] float acceleration = 20;// { get { return acceleration; } }
        [SerializeField] float chargeAcceleration = 50f;
        [Tooltip("The speed the ship will rotate")]
        [SerializeField] float regRotationForce = 2f;
        [SerializeField] float chargeRotationForce = 2f;

        [Space(15)]
        [SerializeField] float waypointDistance = 50f;
        [SerializeField] float playerDistance = 100f;
        [Tooltip("Time in seconds between trajectory calculations")]
        [SerializeField] float calculateInterval = 0.5f;
      
        Rigidbody rbTarget;
        Rigidbody rbSelf;
        Vector3 interceptPoint;

        //Obstacle variables
        Vector3 obstacleAvoidDirection = Vector3.right;
        bool obstacleHit = false;
        float obstacleTimer = 0;
        [SerializeField] float avoidTime = 2f;
        [Tooltip("Size of ray for collision checking. Larger numbers will mean the avoidance is larger")]
        [SerializeField] float raySize = 7.5f;



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
            ChaserPatrolState patrol = new ChaserPatrolState(this, player, waypoints, waypointDistance, playerDistance);

            patrol.AddTransition(Transition.NoHealth, FSMStateID.Dead);
            patrol.AddTransition(Transition.SawPlayer, FSMStateID.Chasing);

            AddFSMState(deadState);
        }
    }
}