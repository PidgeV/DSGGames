using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotInfo : MonoBehaviour
{
    public SNSSTypes.WeaponType weapon;
    [SerializeField] private float spawnTime;
    [SerializeField] private float lifeTime;
    private float lifeTimer = 0;

    public float FireRate { get { return spawnTime; } }

    private void Update()
    {
        if (weapon != SNSSTypes.WeaponType.Laser)
        {
            lifeTimer += Time.deltaTime;

            if (lifeTimer >= lifeTime) Destroy(gameObject);
        }
    }
}
