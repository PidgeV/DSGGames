using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class HealthAndShields : MonoBehaviour
{
	/// <summary> When this GameObjects life or shields change values </summary>
	public delegate void OnLifeChange(float current, float max);
	public OnLifeChange onLifeChange;

	/// <summary> When this GameObjects life or shields change values </summary>
	public delegate void OnShieldChange(float current, float max);
	public OnShieldChange onShieldChange;

	/// <summary> When this GameObject dies </summary>
	public delegate void OnDeath();
	public OnDeath onDeath;


	// The MAX life the ship has
	[SerializeField] float maxLife = 100f;
	public float MaxLife { get { return maxLife; } }

	// The MAX shield the ship has
	[SerializeField] float maxShield = 100f;
	public float MaxShield { get { return maxShield; } }


	// Can we regenerate our shield
	public float regenInterval = 5;
	private float regenDelay = 0;
	public bool CanRegen { get { return Time.realtimeSinceStartup > regenDelay; } }


	public float currentLife;
	public float currentShield;

	public bool Invincible = false;
	public bool DestroyOnDeath = true;


	// The PERCENT of shield that is regenerated per second
	[Range(0, 100)] public int regenSpeed = 5;


	// Start is called before the first frame update
	void Start()
	{
		currentLife = maxLife;
		currentShield = maxShield;

		ShieldProjector shieldProjector = gameObject.GetComponentInChildren<ShieldProjector>();

		if (shieldProjector)
		{
			// So we can take damage when this shield is hit
			shieldProjector.onShieldHit += OnShieldHit;

			// So we can update the color of the shield when we take damage
			onShieldChange += shieldProjector.UpdateShieldPercent;
		}
	}

	// Update is called once per frame
	void Update()
	{
		// If we have more then 0 life we can regen shields
		if (CanRegen && currentLife >= 0)
		{
			// Calculating the amount we need to heal WITH regen Speed
			float amountToHeal = currentShield + (maxShield * regenSpeed / 100f) * Time.deltaTime;

			// Clamp out shield to the max shield
			currentShield = Mathf.Clamp(amountToHeal, 0, maxShield);
		}
	}

	/// <summary>
	/// Reinitializes this scripts values
	/// </summary>
	/// <param name="invokeEvents"> Do you want to call the OnLifeChange events </param>
	public void ResetValues(bool invokeEvents = false)
	{
		currentLife = maxLife;
		currentShield = maxShield;

		if (invokeEvents == true)
		{
			// Invoke the On Life Change Event
			onLifeChange.Invoke(currentLife, maxLife);

			// Invoke the On Shield Change Event
			onShieldChange.Invoke(currentShield, maxShield);
		}
	}

	/// <summary> When the shield it hit </summary>
	public void OnShieldHit(GameObject attacker)
	{
		if (attacker.TryGetComponent<Damage>(out Damage damage))
		{
			// PrintDamage(damage.kineticDamage, damage.energyDamage);
			TakeDamage(damage.KineticDamage, damage.EnergyDamage);
		}
	}

	// Damage the ship
	public void TakeDamage(float kineticDamage, float energyDamage)
	{
		if (Invincible == true)
		{
			// Do nothing
			return;
		}
		else
		{
			regenDelay = Time.realtimeSinceStartup + regenInterval;

			// Damage the shield
			currentShield = Mathf.Clamp(currentShield - energyDamage, 0, maxShield);

			// If we have negative shields we can take it away from your life pool
			if (currentShield == 0)
			{
				currentLife -= kineticDamage;
			}

			// If we are dead cann OnDeath()
			if (currentLife <= 0)
			{
				currentLife = 0;
				HandleDeath();
			}
		}

		// Invoke the On Life Change Event
		if (onLifeChange != null)
		{
			onLifeChange.Invoke(currentLife, maxLife);
		}

		// Invoke the On Shield Change Event		
		if (onShieldChange != null)
		{
			onShieldChange.Invoke(currentShield, maxShield);
		}
	}

	// When life is 0 this is called by TakeDamage or Update
	void HandleDeath()
	{
		currentLife = maxLife;
		currentShield = maxShield;

		// Invoke the On Death Event
		if (onDeath != null)
		{
			onDeath.Invoke();
		}

		if (DestroyOnDeath == true)
		{
			// Destroy the gameobject
			Destroy(gameObject);
		}
		else
		{
			// Respawn the gameobject?
			gameObject.SetActive(false);
		}
	}

	// Heal this GameObjects current life by a given value
	public void Heal(int amountToHeal)
	{
		currentLife += amountToHeal;

		// Clamp that value
		if (currentLife > maxLife) currentLife = maxLife;

		// Invoke the On Life Change Event
		onLifeChange.Invoke(currentLife, maxLife);
	}

	/// <summary> Print out the damage taken </summary>
	public void PrintDamage(float kineticDamage, float energyDamag)
	{
		Debug.Log("Damage Taken:( KineticDamage: " + kineticDamage + ", EnergyDamage: " + energyDamag + ")");
	}
}
