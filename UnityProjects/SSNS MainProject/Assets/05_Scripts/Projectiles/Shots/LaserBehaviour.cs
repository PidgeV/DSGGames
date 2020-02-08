using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBehaviour : MonoBehaviour
{
    [SerializeField] LineRenderer line;
    [SerializeField] float laserLength = 400f;
    [SerializeField] float laserDamagePerSecond = 5f;

    public float Length { get { return laserLength; } }
    public float Damage { get { return laserDamagePerSecond * Time.deltaTime; } }
    public void SetLaser(Vector3 endPosition)
    {
        line.SetPosition(1, endPosition);
    }
}
