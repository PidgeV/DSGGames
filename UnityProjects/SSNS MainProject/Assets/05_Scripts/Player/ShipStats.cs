using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ship Stats", menuName = "Ship/New Ship Stats")]
public class ShipStats : ScriptableObject
{
	// Ship description
	public string shipName;
	public string shipDescription;

	// Ship properties
	// The max values for this ships health and shield
	public float maxHealth = 20f;
	public float maxShield = 15f;

	// The current values for this ships health and shield
	public float currentHealth = 0f;
	public float currentShield = 0f;

	// The speed the ship moves per second
	public float acceleration = 15f;

	// The delay between shots for the players basic weapon
	public float fireRate = 0.1f;

    //Player things
    [Header("Player ship stats")]
	// This ships boost gauge values
	public float boostGauge = 0f;
	public float maxBoostGauge = 5f;

    // The speed the ship moves per second when boosting
    public float boostAcceleration = 30f;

    // The base damage for the player basic gun
    public float baseDamage = 1f; //-----------------------------Why is this not just damage on the bullets and stuff? -Blake botch

    // Reductions of damage for this ship
    public float healthDamageReduction = 0.0f;
    public float shieldDamageReduction = 0.0f;

    // Shield regen properties
    public float shieldRegenPercent = 0.1f;
    public float shieldRegenDelay = 3f;

    // Extra ship abilities
    public bool reserveShield = false;
	public bool shieldBreakEMP = false;
}
