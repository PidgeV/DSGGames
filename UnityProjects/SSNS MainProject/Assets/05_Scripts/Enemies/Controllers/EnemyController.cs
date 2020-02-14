using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public abstract class EnemyController : AdvancedFSM
{
    //[Header("Green: forward direction and collision check.")]
    //[Header("Red: intercept calculation.")]
    //[Header("Blue: Velocity and collision check.")]
    //[SerializeField] private bool debugDraw = false;

    ////[SerializeField] EnemyStats stats;

    ////MAth things for later. To store the dot product
    //float dotProduct;

    //[Space(15)]
    //[SerializeField] LayerMask obstacleLayer;
    //[SerializeField] float collisionCheckDistance = 150f;
    //[SerializeField] float playerDistanceMeters = 100f;

    //[Tooltip("Size of ray for collision checking. Larger numbers will mean the avoidance is larger")]
    //[SerializeField] float raySize = 7.5f;

    //public Rigidbody rbSelf;

    //protected GameObject player;
    //public bool hitPlayer = false;

    ////Obstacle variables
    //Vector3 obstacleAvoidDirection = Vector3.right;
    //bool obstacleHit = false;
    //float obstacleTimer = 0;
    //float avoidTime = 2f;

    //protected abstract void ConstructFSM();

    //protected override void Initialize()
    //{
    //    player = GameObject.FindGameObjectWithTag("Player");

    //    ConstructFSM();
    //}

    //protected override void FSMUpdate()
    //{
    //    //Do this
    //    if (CurrentState != null)
    //    {
    //        CurrentState.Reason();
    //        CurrentState.Act();
    //    }
    //}
    //protected override void FSMFixedUpdate()
    //{
    //    //Guess we needed this
    //}

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.gameObject.Equals(player) && CurrentStateID == FSMStateID.Attacking)
    //    {
    //        hitPlayer = true;
    //    }
    //}

    ////Draw debug rays
    //private void OnDrawGizmos()
    //{
    //    if (debugDraw)
    //    {
    //        Debug.DrawRay(transform.position, transform.forward.normalized * collisionCheckDistance, Color.green); //Forward ray
    //                                                                                                               //Debug.DrawRay(transform.position, interceptPoint - transform.position, Color.red); // Intercept point
    //        if (rbSelf)
    //        {
    //            //Vector3 dir = rbSelf.velocity;
    //            Debug.DrawRay(transform.position, rbSelf.velocity.normalized * collisionCheckDistance, Color.blue); //Velocity ray
    //        }
    //    }
    //}

    //public bool PlayerInVision()
    //{
    //    Vector3 dir = player.transform.position - transform.position;
    //    Ray ray = new Ray(transform.position, dir);
    //    RaycastHit hitInfo;
    //    LayerMask layerMask = LayerMask.GetMask("Obstacles");
    //    layerMask += LayerMask.GetMask("Player"); //Add player layer to obstacles
    //    layerMask += LayerMask.GetMask("Enemies");

    //    Physics.Raycast(ray, out hitInfo, Mathf.Infinity, layerMask);

    //    if (hitInfo.collider != null)
    //    {
    //        Debug.Log(hitInfo.collider.gameObject.name);
    //        if (hitInfo.collider.gameObject.Equals(player.gameObject))
    //        {
    //            return true;
    //        }
    //    }
    //    return false;
    //}

    //public bool AvoidObstacles(ref Vector3 dir)
    //{
    //    RaycastHit hitInfo;

    //    //Check direction facing
    //    if (Physics.SphereCast(transform.position, RaySize, transform.forward.normalized,
    //        out hitInfo, CollisionCheckDistance, ObstacleLayer) /*||
    //        Physics.SphereCast(controller.transform.position, controller.RaySize, controller.rbSelf.velocity.normalized,
    //        out hitInfo, controller.CollisionCheckDistance, controller.ObstacleLayer)*/)
    //    {
    //        // Get the desired direction we need to move to move around  the obstacle. Transform to world co-ordinates (gets the obstacleMoveDirection wrt the current foward direction).
    //        Vector3 turnDir = transform.TransformDirection(hitInfo.normal * 2);
    //        turnDir.Normalize();

    //        dir += turnDir;
    //        return true;
    //    }
    //    return false;
    //}

    ////Moves
    //public void Move(Vector3 destination)
    //{
    //        //Calculate direction
    //        Vector3 direction = transform.forward; // sets forward
    //        direction.Normalize();

    //        if (AvoidObstacles(ref direction)) // will change direction towards the right if an obstacle is in the way
    //        {
    //            obstacleHit = true;
    //        }

    //        //Rotation
    //        if (!obstacleHit && obstacleTimer == 0)
    //        {
    //            direction = destination - transform.position; // sets desired direction to target intercept point
    //        }
    //        else
    //        {
    //            //if obstacles, ignore desired direction and move to the right of obstacles
    //            obstacleTimer += Time.deltaTime;
    //            if (obstacleTimer > avoidTime)
    //            {
    //                obstacleTimer = 0;
    //                obstacleHit = false;
    //            }
    //        }


    //        Vector3 newDir = Vector3.RotateTowards(transform.forward, direction, RegRotationForce * Time.deltaTime, 0);
    //        Quaternion rot = Quaternion.LookRotation(newDir);
    //        transform.rotation = Quaternion.Lerp(transform.rotation, rot, RegRotationForce * Time.deltaTime);

    //        //Movement
    //        rbSelf.AddForce(transform.forward.normalized * Acceleration, ForceMode.Acceleration); // move regular speed if obstacle is in the way or player is not target
    //}

    //public float CollisionCheckDistance { get { return collisionCheckDistance; } }
    //public float PlayerDistance { get { return playerDistanceMeters; } }
    //public LayerMask ObstacleLayer { get { return obstacleLayer; } }
    //public float RaySize { get { return raySize; } }
    //public GameObject Player { get { return player; } }
}