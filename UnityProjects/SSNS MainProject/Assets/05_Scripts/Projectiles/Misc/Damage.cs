using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// When two gameObjects collide they look for this script
[RequireComponent(typeof(Collider))]
public class Damage : MonoBehaviour
{
    [SerializeField] bool destroyOnHit = false;
    // The damage whatever collides with the gameObject holding this script should take
    [SerializeField] int kineticDamage = 5;
    [SerializeField] int energyDamage = 5;

    public int KineticDamage { get { return kineticDamage; } }
    public int EnergyDamage { get { return energyDamage; } }

    public void ChangeDamage(int kinetic, int energy)
    {
        kineticDamage = kinetic;
        energyDamage = energy;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out HealthAndShields hpTemp))
        {
            hpTemp.TakeDamage(kineticDamage, energyDamage);
        }

        if (destroyOnHit)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out HealthAndShields hpTemp))
        {
            hpTemp.TakeDamage(kineticDamage, energyDamage);
        }

        if(destroyOnHit)
        {
            Destroy(gameObject);
        }
    }
}
