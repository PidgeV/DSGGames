using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterPatrolState : PatrolState
{
    public FighterPatrolState(FighterController enemyController, bool randomizePoint = false) : base(enemyController, randomizePoint)
    {
    }
}