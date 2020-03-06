using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CargoPatrolState : PatrolState<CargoController>
{
    private bool reachedShip;

    public CargoPatrolState(CargoController enemyController) : base(enemyController)
    {
    }


    //Do this always
    public override void Act()
    {
        Move(controller.CapitalShip.transform);
    }

    //Initialize on entering state
    public override void EnterStateInit()
    {
        //Debug.Log("Patrolling");
        controller.hitPlayer = false;
    }


    public override void Reason()
    {
        //Check distance to the capital ship
        if (Vector3.Distance(controller.transform.position, controller.CapitalShip.transform.position) < controller.WaypointDistance && !reachedShip)
        {
            //Do reaching capitol ship things
            reachedShip = true;
            Debug.Log("reached the ship");
        }

        //If dead transition to dead
        if (controller.Health.IsDead)
        {
            controller.PerformTransition(Transition.NoHealth);
        }
    }
}