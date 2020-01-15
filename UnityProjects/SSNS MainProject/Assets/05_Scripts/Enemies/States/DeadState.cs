using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Complete
{
    /// <summary>
    /// This state is meant to be used for an enemie once it has been killed
    /// </summary>
    public class DeadState : FSMState
    {
        /// <summary> This gameobject holds the prefab for the destroyed enemy </summary>
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
            //Spawn the destroyed version of the enemy
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
}