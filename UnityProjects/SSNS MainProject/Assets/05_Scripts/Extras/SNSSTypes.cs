using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SNSSTypes
{
    public enum PlayerRole
    {
        Gunner,
        Pilot,
        None
    };

    public enum NodeType
    {
        Tutorial,
        Reward,
        Boss
    }

    public enum GameState
    {
        NODE_TRANSITION,
        BATTLE,
        BATTLE_END,
        NODE_SELECTION,
        PAUSE,
        GAME_END
    }

    public enum NodeEventType
    {
        Tutorial,
        Random,
        MiniBoss,
        Boss
    }

	public enum FadeType
	{
		NO_FADE,
		HALF_FADE,
		FULL_FADE
	}

	//RegularShot: Normal
	//EnergyShot: Regular shot, but for shields
	//ChargeShot: Charges over course of 1.5 seconds, then fires
	//Missiles: Spawns many misiles to hit many enemies (think whistlin birds)
	//LaserShot: Long, straight beam, near instant, aoe from origin

	public enum WeaponType { Regular, Energy, Laser, Charged, Missiles, End }
}
