using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ship Stats", menuName = "Ship/New Ship Stats")]
[System.Serializable]
public class ShipStats : ScriptableObject
{
	// Ship description
	[SerializeField] private string shipName;
    [SerializeField] private string shipDescription;

	// Ship properties
	// The max values for this ships health and shield
	public float maxHealth = 20f;
	public float maxShield = 15f;

	// Acceleration and deacceleration of the ship's speed
	public float shipAcceleration = .3f;
	public float shipDeceleration = .01f;

    // Acceleration and deacceleration for the ship's rotation
    public float shipRotAcceleration = 5;
    public float shipRotDeceleration = 2;

    // Min and max thruster speed of the ship
	public float thrustSpeed = 50f;

	// The speed the ship moves per second when boosting
	public float boostSpeed = 30f;

	// The speed of strafing
	public float strafeSpeed = 30f;

	// The speed that the ship rotates
	public float rotationSpeed = 5f;

	// The delay between shots for the players basic weapon
	public float fireRate = 0.1f;

	// The base damage for the player basic gun
	public float baseDamage = 1f;

	// Reductions of damage for this ship
	public float healthDamageReduction = 0.0f;
	public float shieldDamageReduction = 0.0f;

	// Shield regen properties
	public float shieldRegenPercent = 0.1f;
	public float shieldRegenDelay = 3f;

	// This ships boost gauge values
	public float maxBoostGauge = 5f;

	// The amount boosting uses the meter every second
	public float boostGaugeConsumeAmount = 2.5f;

	// The amount boosting recharges the meter every second
	public float boostGaugeRechargeAmount = 1.5f;

	// Extra ship abilities
	public bool reserveShield = false;
	public bool shieldBreakEMP = false;

    public string ShipName { get { return shipName; } }
    public string ShipDescription { get { return shipDescription; } }
}
