using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// When two gameObjects collide they look for this script
[RequireComponent(typeof(Collider))]
public class Damage : MonoBehaviour
{
    [SerializeField] bool destroyOnHit = false;
    [SerializeField] bool constantCollisionDamage = false;
    // The damage whatever collides with the gameObject holding this script should take
    public int kineticDamage = 5;
	public int energyDamage = 5;

    [Space(10)]
    [SerializeField] GameObject hitSoundObject;

    public int KineticDamage { get { return kineticDamage; } }
    public int EnergyDamage { get { return energyDamage; } }

    public void ChangeDamage(int kinetic, int energy)
    {
        kineticDamage = kinetic;
        energyDamage = energy;
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Spawn the hit sound Object and parent to what it hit. Do this in case the object is destroyed on hitting things
        if (hitSoundObject)
            Instantiate(hitSoundObject, transform.position, Quaternion.identity, collision.transform);

        //Apply damage to things it hits
        if (collision.gameObject.TryGetComponent(out HealthAndShields hpTemp))
        {
            hpTemp.TakeDamage(kineticDamage, energyDamage);
        }

        if (destroyOnHit)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        //Apply damage to things it hits
        if (collision.gameObject.TryGetComponent(out HealthAndShields hpTemp) && constantCollisionDamage)
        {
            hpTemp.TakeDamage(kineticDamage, energyDamage);
        }
    }

    private void OnTriggerEnter(Collider other)
    {

        //Spawn the hit sound Object and parent to what it hit. Do this in case the object is destroyed on hitting things
        if (hitSoundObject)
            Instantiate(hitSoundObject, other.transform);

        //Apply damage to things it hits
        if (other.gameObject.TryGetComponent(out HealthAndShields hpTemp))
        {
            hpTemp.TakeDamage(kineticDamage * Time.deltaTime, energyDamage * Time.deltaTime);
        }

        if(destroyOnHit)
        {
            Destroy(gameObject);
        }
    }
}
