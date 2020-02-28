using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : ShipStats
{
    [Space(15)]
    [SerializeField] LayerMask obstacleLayer = 1 << 8 | 9 | 12;
    [SerializeField] float collisionDistanceCheck = 150f;

    [Space(15)]
    [SerializeField] float waypointDistanceCheck = 50f;
    [SerializeField] float playerDistanceCheck = 100f;

    [Tooltip("Size of ray for collision checking. Larger numbers will mean the avoidance is larger")]
    [SerializeField] float raySize = 7.5f;

    public LayerMask ObstacleLayer { get { return obstacleLayer; } }
    public float RaySize { get { return raySize; } }
    public float CollisionDistance { get { return collisionDistanceCheck; } }
    public float WaypointDistance { get { return waypointDistanceCheck; } }
    public float PlayerDistance { get { return playerDistanceCheck; } }
}
