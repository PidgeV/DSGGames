using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ship Stats", menuName = "Ship/New Ship Stats")]
public abstract class ShipStats : ScriptableObject
{
	// Ship description
	[SerializeField] private string shipName;
	[SerializeField] private string shipDescription;

	// Ship properties
	// The max values for this ships health and shield
	public float maxHealth = 100f;
	public float maxShield = 100f;

	public float normalSpeed = 30f;

	public float maxSpeed = 50f;

	public float rotationSpeed = 5f;

	public float fireRate = 0.1f;

	public float baseDamage = 1f;

	// Shield regen properties
	[Range(0, 100)] public int shieldRegenPercent = 5;
	public float shieldRegenInterval = 5;

	public string ShipName { get { return shipName; } }
	public string ShipDescription { get { return shipDescription; } }
}
