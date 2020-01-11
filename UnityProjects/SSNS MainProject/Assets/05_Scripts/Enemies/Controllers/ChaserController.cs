using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Complete
{
    [RequireComponent(typeof(Rigidbody))]
    public class ChaserController : AdvancedFSM
    {
        [SerializeField] GameObject destroyedPrefab;
        [SerializeField] GameObject[] waypoints;
      
        [Header("Green: forward direction and collision check.")]
        [Header("Red: intercept calculation.")]
        [Header("Blue: Velocity and collision check.")]
        [SerializeField] bool debugDraw = false;

        [SerializeField] ShipStats stats;

        //MAth things for later. To store the dot product
        float dotProduct;

        [Space(15)]
        [SerializeField] LayerMask obstacleLayer;
        [SerializeField] float collisionCheckDistance = 150f;

        [Space(15)]
        [SerializeField] float waypointDistance = 50f;
        [SerializeField] float playerDistance = 100f;
        [Tooltip("Time in seconds between trajectory calculations")]
        [SerializeField] float calculateInterval = 0.5f;
      
        Rigidbody rbTarget;
        public Rigidbody rbSelf;
        Vector3 interceptPoint;

        [Tooltip("Size of ray for collision checking. Larger numbers will mean the avoidance is larger")]
        [SerializeField] float raySize = 7.5f;

        GameObject target;

        Player player;

        private void ConstructFSM()
        {
            DeadState deadState = new DeadState(this, destroyedPrefab);
            ChaserPatrolState patrol = new ChaserPatrolState(this, player, waypoints, waypointDistance, playerDistance, true);

            patrol.AddTransition(Transition.NoHealth, FSMStateID.Dead);
            patrol.AddTransition(Transition.SawPlayer, FSMStateID.Chasing);

            AddFSMState(patrol);
            AddFSMState(deadState);
        }

        protected override void Initialize()
        {
            //Pretty self explainatory
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
            ConstructFSM();

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
        protected override void FSMFixedUpdate()
        {
            //Guess we needed this
        }

        //Draw debug rays
        private void OnDrawGizmos()
        {
            if (debugDraw)
            {
                Debug.DrawRay(transform.position, transform.forward.normalized * collisionCheckDistance, Color.green); //Forward ray
                //Debug.DrawRay(transform.position, interceptPoint - transform.position, Color.red); // Intercept point
                if (rbSelf)
                {
                    //Vector3 dir = rbSelf.velocity;
                    Debug.DrawRay(transform.position, rbSelf.velocity.normalized * collisionCheckDistance, Color.blue); //Velocity ray
                }
            }
        }

        public float CollisionCheckDistance { get { return collisionCheckDistance; } }
        public LayerMask ObstacleLayer { get { return obstacleLayer; } }
        public float RaySize {  get { return raySize; } }
        public float RegRotationForce { get { return stats.regRotationForce; } }
        public float ChargeRotationForce {  get { return stats.chargeRotationForce; } }
        public float Acceleration { get { return stats.acceleration; } }
        public float ChargeAcceleration { get { return stats.chargeAcceleration; } }
    }
}