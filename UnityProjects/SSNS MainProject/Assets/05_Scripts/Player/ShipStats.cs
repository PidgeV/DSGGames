using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ship Stats", menuName = "Ship/New Ship Stats")]
public class ShipStats : ScriptableObject
{
	// Ship description
	public string ShipName;
	public string ShipDescription;

	// Ship properties
	public float MaxHealth = 10;
	public float MaxShield = 10;
	public float CurrentHealth = 0;
	public float CurrentShield = 0;

	public float ShipSpeed = 15f;
	public float BoostSpeed = 30f;
	public float FireRate = 0.1f;

	public float BaseDamage = 1f;

	public float HealthDamageReduction = 0.0f;
	public float ShieldDamageReduction = 0.0f;

	public float ShieldRegenPercent = 0.05f;
	public float ShieldRegenDelay = 2f;

	public float BoostGauge = 5f;
	public float MaxBoostGauge = 5f;

	public bool ReserveShield = false;
	public bool ShieldBreakEMP = false;
}
