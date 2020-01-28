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

    public enum ShotType { Regular, Energy, Laser, Charged, Missiles }

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
}
