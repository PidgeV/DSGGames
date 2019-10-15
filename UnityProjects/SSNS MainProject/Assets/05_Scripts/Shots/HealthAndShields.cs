using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// NOTE -- To work you should have a collider on this gameobject WITH IsTrigger set to true
//		   You also need a Rigidbody that is NOT kinematic

[RequireComponent(typeof(Rigidbody))]
public class HealthAndShields : MonoBehaviour
{
	[Space(10)]
	// The MAX life the ship has
	public float maxLife = 100f;

	// The MAX shield the ship has
	public float maxShield = 100f;

	[Space(10)]
	public float life;
	public float shield;

	[Space(10)]
	[Range(1, 99)]
	// The PERCENT of damage that is reduced when taking a hit to LIFE 
	public float armor = 1f;

	[Range(1, 99)]
	// The PERCENT of shield that is regenerated per second
	public float regenSpeed = 1f;

	// Start is called before the first frame update
	void Start()
	{
		life = maxLife;
		shield = maxShield;
	}

	// Update is called once per frame
	void Update()
	{
		// If we have more then 0 life we can regen shields
		if (life > 0)
		{
			// Calculating the amount we need to heal WITH regen Speed
			float amountToHeal = shield + (maxShield * regenSpeed / 100f) * Time.deltaTime;

			// Clamp out shield to the max shield
			shield = Mathf.Clamp(amountToHeal, -Mathf.Infinity, maxShield);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		// We get the DAMAGE component from a gameobject
		Damage hit = other.GetComponent<Damage>();

		// If it doesn't have one we move on
		if (hit)
		{
			TakeDamage(hit.damage);
		}
	}

	// Damage the ship
	void TakeDamage(float damage)
	{
		// Damage the shield
		shield -= damage;

		// If we have negative shields we can take it away from your life pool
		if (shield < 0)
		{
			// Apply the armor reduction
			life += -Mathf.Abs(shield / armor);
			shield = 0;
		}

		// If we are dead cann OnDeath()
		if (life <= 0)
		{
			life = 0;
			OnDeath();
		}
	}

	// When life is 0 this is called by TakeDamage()
	void OnDeath()
	{
		Destroy(gameObject);
	}
}
