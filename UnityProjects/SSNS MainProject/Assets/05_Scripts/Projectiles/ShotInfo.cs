using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotInfo : MonoBehaviour
{
    public SNSSTypes.WeaponType weapon;
    [SerializeField] private float fireRate;
    [Tooltip("Life in seconds, 0 for infinite life")]
    [Range(0, 120)]
    [SerializeField] private float lifeTime;
    [SerializeField] private float speed;
    private float lifeTimer = 0;

    public float FireRate { get { return fireRate; } }
    public float Speed { get { return speed; } }

    private void Update()
    {
        if (weapon != SNSSTypes.WeaponType.Laser || lifeTime != 0)
        {
            lifeTimer += Time.deltaTime;

            if (lifeTimer >= lifeTime) Destroy(gameObject);
        }
    }
}
