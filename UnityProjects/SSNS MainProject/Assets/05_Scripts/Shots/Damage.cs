using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// When two gameObjects collide they look for this script
public class Damage : MonoBehaviour
{
	// The damage whatever collides with the gameObject holding this script should take
	public float damage = 5f;

    public void ChangeDamage(float newDamage)
    {
        damage = newDamage;
    }
}
