﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Player Stats", menuName = "Ship/New Player Stats")]
[System.Serializable]
public class PlayerStats : ShipStats
{
	// Acceleration and deacceleration of the ship's speed
	public float shipAcceleration = .3f;
	public float shipDeceleration = .01f;

    // Acceleration and deacceleration for the ship's rotation
    public float shipRotAcceleration = 5;
    public float shipRotDeceleration = 2;

	// The speed of strafing
	public float strafeSpeed = 30f;

	// Reductions of damage for this ship
	public float healthDamageReduction = 0.0f;
	public float shieldDamageReduction = 0.0f;

	// This ships boost gauge values
	public float maxBoostGauge = 5f;

	// The amount boosting uses the meter every second
	public float boostGaugeConsumeAmount = 2.5f;

	// The amount boosting recharges the meter every second
	public float boostGaugeRechargeAmount = 1.5f;

	// Extra ship abilities
	public bool reserveShield = false;
	public bool shieldBreakEMP = false;
}
