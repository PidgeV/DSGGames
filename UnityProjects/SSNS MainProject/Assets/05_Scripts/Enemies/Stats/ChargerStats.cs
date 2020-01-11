using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargerStats : MonoBehaviour
{
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
    public float regRotationForce = 2f;

    public float chargeAcceleration = 50f;
    public float chargeRotationForce = 2f;
}
