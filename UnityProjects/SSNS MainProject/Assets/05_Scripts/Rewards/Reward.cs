using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Reward", menuName = "Rewards/New Reward")]
public class Reward : ScriptableObject
{
	public string RewardName = "";
	public string RewardDescription = "";

	// the value of the reward
	public float value = 0.0f;

	// The type of reward this is
	public RewardsType type = RewardsType.NoReward;

	/// <summary>
	/// Use this reward on a gameobject
	/// </summary>
	public void UseReward(GameObject target)
	{
		if (type == RewardsType.NoReward) {
			return;
		}


	}
}

// The different rewards you can have you can have
public enum RewardsType
{
	// The Default
	NoReward,

	// Flat increases
	ExtraHealth,
	ExtraShield,
	ExtraDamage,

	ShipSpeed,
	BoostSpeed,

	FiringSpeed,

	// Damage reduction
	HealthDamageReduction,
	ShieldDamageReduction,

	// Fast heals
	FullHealthRestore,

	// Shield Regen
	IncreasedRegenSpeed,
	RegenDelayReduction,

	// Boost gauge
	BoostGaugeInncrase,

	// Special effects
	ReserveShield,
	ShieldBreakEMP
}
