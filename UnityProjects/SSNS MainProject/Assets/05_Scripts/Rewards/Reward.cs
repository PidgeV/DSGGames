using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

[CreateAssetMenu(fileName = "New Reward", menuName = "Rewards/New Reward")]
public class Reward : ScriptableObject
{
	public string rewardName = "";
	public string rewardDescription = "";

    public FieldInfo statField;

	// the value of the reward
	public float value = 0.0f;

	/// <summary>
	/// Use this reward on a gameobject
	/// </summary>
	public void UseReward(ShipStats target)
	{
        if (statField.FieldType == typeof(bool))
        {
            statField.SetValue(target, true);
        }
        else if (statField.FieldType == typeof(int))
        {
            statField.SetValue(target, ((int)statField.GetValue(target)) + value);
        }
        else if (statField.FieldType == typeof(float))
        {
            statField.SetValue(target, ((float)statField.GetValue(target)) + value);
        }
	}
}
