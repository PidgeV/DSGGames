using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadState : FSMState
{
    private GameObject destroyed;
    private AdvancedFSM controller;
    public DeadState(AdvancedFSM enemyController, GameObject destroyedPrefab)
    {
        destroyed = destroyedPrefab;
        controller = enemyController;
        stateID = FSMStateID.Dead;
    }

    public override void Act()
    {
        //Spawn
        if (destroyed != null)
        {
            Transform trans = controller.gameObject.transform;
            GameObject tempObj = Object.Instantiate(destroyed, trans.position, trans.rotation);

            //Set velocity
            Rigidbody rb = tempObj.GetComponent<Rigidbody>();
            if (rb)
            {
                rb.velocity = controller.gameObject.GetComponent<Rigidbody>().velocity;
            }
            else
            {
                tempObj.AddComponent(typeof(Rigidbody));
                rb = tempObj.GetComponent<Rigidbody>();
                rb.velocity = controller.gameObject.GetComponent<Rigidbody>().velocity;
            }
        }

        //Destroy original object
        Object.Destroy(controller.gameObject);
    }

    public override void Reason()
    {
        //Do nothing
    }
}