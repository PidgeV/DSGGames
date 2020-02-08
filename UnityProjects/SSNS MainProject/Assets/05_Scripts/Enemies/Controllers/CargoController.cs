using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Complete
{
    [RequireComponent(typeof(Rigidbody))]
    public class CargoController : AdvancedFSM
    {        
        [Header("Green: forward direction and collision check.")]
        [Header("Red: intercept calculation.")]
        [Header("Blue: Velocity and collision check.")]
        [SerializeField] bool debugDraw = false;

        //[SerializeField] ChargerStats stats;

        //MAth things for later. To store the dot product
        float dotProduct;

        [Space(15)]
        [SerializeField] LayerMask obstacleLayer;
        [SerializeField] float collisionCheckDistance = 150f;

        [Space(15)]
        [SerializeField] float capitalShipDistanceMeters = 50f;
      
        public Rigidbody rbSelf;

        [Tooltip("Size of ray for collision checking. Larger numbers will mean the avoidance is larger")]
        [SerializeField] float raySize = 7.5f;

        [SerializeField] float regRotationForce;
        [SerializeField] float acceleration;
        [SerializeField] float currentHealth;
        

        GameObject player;
        GameObject capitalShip;
        public bool hitPlayer = false;

        private void ConstructFSM()
        {
            DeadState deadState = new DeadState(this);
            CargoPatrolState patrol = new CargoPatrolState(this, player, capitalShip);

            patrol.AddTransition(Transition.NoHealth, FSMStateID.Dead);
            patrol.AddTransition(Transition.SawPlayer, FSMStateID.Attacking); //Change this

            AddFSMState(patrol);
            AddFSMState(deadState);
        }

        protected override void Initialize()
        {
            //Pretty self explainatory
            player = GameObject.FindGameObjectWithTag("Player");
            capitalShip = GameObject.FindGameObjectWithTag("CapitalShip");


        //stats.currentHealth = stats.maxHealth;
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

        private void OnCollisionEnter(Collision collision)
        {
            if(collision.gameObject.Equals(player) && CurrentStateID == FSMStateID.Attacking)
            {
                hitPlayer = true;
            }
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

        //Getters
        public float CapitalShipDistance { get { return capitalShipDistanceMeters; } }
        public float CollisionCheckDistance { get { return collisionCheckDistance; } }
        public LayerMask ObstacleLayer { get { return obstacleLayer; } }
        public float RaySize {  get { return raySize; } }
        public float RegRotationForce { get { return regRotationForce; } }
        public float Acceleration { get { return acceleration; } }
        public float Health {  get { return currentHealth; } }
        public GameObject Player { get { return player; } }
    }
}