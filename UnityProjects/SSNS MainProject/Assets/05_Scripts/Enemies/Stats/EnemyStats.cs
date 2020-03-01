using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Stats", menuName = "Ship/New Enemy Stats")]
public class EnemyStats : ShipStats
{
    [Space(15)]
    [SerializeField] LayerMask obstacleLayer = 1 << 8 | 9 | 12;

    [Tooltip("Size of ray for collision checking. Larger numbers will mean the avoidance is larger")]
    [SerializeField] float raySize = 7.5f;

    [SerializeField] float accuracy;

    [Space(15)]
    [SerializeField] float collisionDistanceCheck = 150f;
    [SerializeField] float waypointDistanceCheck = 50f;
    [SerializeField] float playerDistanceCheck = 100f;
    [SerializeField] float attackDistanceCheck = 80f;
    [SerializeField] float patrolDistanceCheck = 200f;

    public LayerMask ObstacleLayer { get { return obstacleLayer; } }
    public float RaySize { get { return raySize; } }
    public float Accuracy { get { return accuracy; } }
    public float CollisionDistance { get { return collisionDistanceCheck; } }
    public float WaypointDistance { get { return waypointDistanceCheck; } }
    public float AttackDistance { get { return attackDistanceCheck; } }
    public float PatrolDistance { get { return patrolDistanceCheck; } }
    public float PlayerDistance { get { return playerDistanceCheck; } }
}
