using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotInfo : MonoBehaviour
{
    public SNSSTypes.WeaponType weapon;
    [SerializeField] private float fireRate;
    [SerializeField] private float lifeTime;
    [SerializeField] private float speed;
    private float lifeTimer = 0;

    public float FireRate { get { return fireRate; } }
    public float Speed { get { return speed; } }

    private void Update()
    {
        if (weapon != SNSSTypes.WeaponType.Laser)
        {
            lifeTimer += Time.deltaTime;

            if (lifeTimer >= lifeTime) Destroy(gameObject);
        }
    }
}
