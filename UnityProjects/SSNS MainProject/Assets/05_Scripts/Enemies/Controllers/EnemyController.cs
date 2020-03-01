using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public abstract class EnemyController : AdvancedFSM
{
    public Transform[] waypoints;

    [SerializeField] private EnemyStats myStats;

    [Header("Green: forward direction and collision check.")]
    [Header("Red: intercept calculation.")]
    [Header("Blue: Velocity and collision check.")]
    [SerializeField] bool debugDraw = false;

    private Rigidbody rbSelf;
    private HealthAndShields health;

    //MAth things for later. To store the dot product
    float dotProduct;

    private Vector3 spawnpoint;

    private Vector3 spawnDestination;

    protected abstract void ConstructFSM();

    protected override void Initialize()
    {
        ConstructFSM();

        TryGetComponent(out rbSelf);
        TryGetComponent(out health);
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

    public bool AvoidObstacles(ref Vector3 dir)
    {
        bool hit = false;
        RaycastHit hitInfo;

        //Check direction facing
        if (Physics.SphereCast(transform.position, RaySize, transform.forward.normalized, out hitInfo, CollisionDistance, ObstacleLayer))/* || Physics.SphereCast(controller.transform.position, controller.RaySize, controller.rbSelf.velocity.normalized, out hitInfo, controller.CollisionCheckDistance, controller.ObstacleLayer))*/
        {
            // Get the desired direction we need to move to move around  the obstacle. Transform to world co-ordinates (gets the obstacleMoveDirection wrt the current foward direction).
            Vector3 turnDir = transform.TransformDirection(hitInfo.normal);
            turnDir.Normalize();

            dir += turnDir;
            hit = true;
        }
        if (Physics.SphereCast(transform.position, RaySize, rbSelf.velocity.normalized, out hitInfo, CollisionDistance, ObstacleLayer))
        {
            // Get the desired direction we need to move to move around  the obstacle. Transform to world co-ordinates (gets the obstacleMoveDirection wrt the current foward direction).
            Vector3 turnDir = transform.TransformDirection(hitInfo.normal);
            turnDir.Normalize();

            dir += turnDir;
            hit = true;
        }
        return hit;
    }

    public bool PlayerInVision()
    {
        Vector3 dir = Player.transform.position - transform.position;
        Ray ray = new Ray(transform.position, dir);
        RaycastHit hitInfo;
        LayerMask layerMask = LayerMask.GetMask("Obstacles");
        layerMask += LayerMask.GetMask("Player"); //Add player layer to obstacles
        layerMask += LayerMask.GetMask("Enemies"); //Add player layer to obstacles

        Physics.SphereCast(ray, myStats.RaySize / 2, out hitInfo, Mathf.Infinity, layerMask);

        if (hitInfo.collider != null)
        {
            //Debug.Log(hitInfo.collider.gameObject.name);
            if (hitInfo.collider.gameObject.Equals(Player))
            {
                return true;
            }
        }
        return false;
    }

    //Draw debug rays
    private void OnDrawGizmos()
    {
        if (debugDraw)
        {
            Debug.DrawRay(transform.position, transform.forward.normalized * CollisionDistance, Color.green); //Forward ray
                                                                                                              //Debug.DrawRay(transform.position, interceptPoint - transform.position, Color.red); // Intercept point
            if (rbSelf)
            {
                //Vector3 dir = rbSelf.velocity;
                Debug.DrawRay(transform.position, rbSelf.velocity.normalized * CollisionDistance, Color.blue); //Velocity ray
            }
        }
    }

    public GameObject Player { get { return GameManager.Instance.Player; } }
    public HealthAndShields Health { get { return health; } }
    public EnemyStats Stats { get { return myStats; } }
    public Rigidbody Rigid { get { return rbSelf; } }
    public LayerMask ObstacleLayer { get { return myStats.ObstacleLayer; } }
    public float RaySize { get { return myStats.RaySize; } }
    public float CollisionDistance { get { return myStats.CollisionDistance; } }
    public float WaypointDistance { get { return myStats.WaypointDistance; } }
    public float PlayerDistance { get { return myStats.PlayerDistance; } }
    public Vector3 Spawn { get { return spawnpoint; } set { spawnpoint = value; } }
    public Vector3 SpawnDestination { get { return spawnDestination; } set { spawnDestination = value; } }
}