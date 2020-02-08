using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropPickup : MonoBehaviour
{
    [Range(0,100)]
    [SerializeField] float percentile = 10f;
    [SerializeField] Pickup[] pickups;

    public void AttemptPickupSpawn()
    {
        if (Random.Range(1, 101) < percentile)
        {
            int rand = Random.Range(0, pickups.Length);
            Instantiate(pickups[rand].gameObject, transform.position, Quaternion.identity);
        }
    }
}
