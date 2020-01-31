using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// When two gameObjects collide they look for this script
[RequireComponent(typeof(Collider))]
public class Damage : MonoBehaviour
{
	// The damage whatever collides with the gameObject holding this script should take
	public int kineticDamage = 5;
    public int energyDamage = 5;

    public void ChangeDamage(int kinetic, int energy)
    {
        kineticDamage = kinetic;
        energyDamage = energy;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.TryGetComponent(out HealthAndShields hpTemp))
        {
            hpTemp.TakeDamage(kineticDamage, energyDamage);
        }
    }

    //private void OnCollisionStay(Collision collision)
    //{
    //    if (collision.gameObject.TryGetComponent(out HealthAndShields hpTemp))
    //    {
    //        hpTemp.TakeDamage(kineticDamage, energyDamage);
    //    }
    //}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out HealthAndShields hpTemp))
        {
            hpTemp.TakeDamage(kineticDamage, energyDamage);
        }
    }
}
