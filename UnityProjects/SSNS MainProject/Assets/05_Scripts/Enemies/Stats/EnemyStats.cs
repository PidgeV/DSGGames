using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Stats", menuName = "Ship/Enemy Stats")]
public class EnemyStats : ShipStats
{
    [SerializeField] public float attackShipSpeed = 50f;

    [SerializeField] public float attackRotationSpeed = 2f;

    [SerializeField] public float accuracy;
}
