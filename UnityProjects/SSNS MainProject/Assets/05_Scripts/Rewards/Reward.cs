using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Reward", menuName = "Rewards/New Reward")]
public class Reward : ScriptableObject
{
	public string rewardName = "";
	public string rewardDescription = "";

	// the value of the reward
	public float value = 0.0f;

	// The type of reward this is
	public RewardsType type = RewardsType.NoReward;

	/// <summary>
	/// Use this reward on a gameobject
	/// </summary>
	public void UseReward(ShipStats target)
	{
		Debug.Log("You are applying the reward " + type.ToString() + " to the player!");

		if (type == RewardsType.NoReward) {
			return;
		}
		else
		{
			switch (type) {

				case RewardsType.ExtraHealth:
					target.MaxHealth += value;
					target.CurrentHealth += value;
					break;

				case RewardsType.ExtraShield:
					target.MaxShield += value;
					target.CurrentShield += value;
					break;

				case RewardsType.ExtraDamage:
					target.BaseDamage += value;
					break;

				case RewardsType.ShipSpeed:
					target.ShipSpeed += value;
					break;

				case RewardsType.BoostSpeed:
					target.BoostSpeed += value;
					break;

				case RewardsType.FiringSpeed:
					target.FireRate += value;
					break;

				case RewardsType.HealthDamageReduction:
					target.HealthDamageReduction += value;
					break;

				case RewardsType.ShieldDamageReduction:
					target.ShieldDamageReduction += value;
					break;

				case RewardsType.FullHealthRestore:
					target.CurrentHealth = target.MaxHealth;
					break;

				case RewardsType.IncreasedRegenSpeed:
					target.ShieldRegenPercent += value;
					break;

				case RewardsType.RegenDelayReduction:
					target.ShieldRegenDelay += value;
					break;

				case RewardsType.BoostGaugeInncrase:
					target.MaxBoostGauge += value;
					break;

				case RewardsType.ReserveShield:
					target.ReserveShield = true;
					break;

				case RewardsType.ShieldBreakEMP:
					target.ShieldBreakEMP = true;
					break;
			}
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
