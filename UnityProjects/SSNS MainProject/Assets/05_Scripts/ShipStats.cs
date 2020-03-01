using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ShipStats : ScriptableObject
{
	// Ship description
	[SerializeField] private string shipName;
	[SerializeField] private string shipDescription;

	// Ship properties
	// The max values for this ships health and shield
	public float maxHealth = 100f;
	public float maxShield = 100f;

	public float shipSpeed = 30f;

	public float rotationSpeed = 5f;

	public float fireRate = 0.1f;

	// Shield regen properties
	[Range(0, 100)] public float shieldRegenPercent = 5;
	public float shieldRegenInterval = 5;

	public string ShipName { get { return shipName; } }
	public string ShipDescription { get { return shipDescription; } }
}
