using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// NOTE -- This script shoud not be on anything by default
//           It is given to an Input Object after the player selects their role

public class ShipInput : MonoBehaviour
{
    public PilotController pilotController;

    Vector3 lastInput = new Vector3();

    bool boosting = false;

    private void Awake()
    {
        pilotController = FindObjectOfType<PilotController>();
    }

    private void Update()
    {
        pilotController.Move(lastInput * Time.deltaTime);
        pilotController.Boost(boosting);
        pilotController.SetShipTransfrom(lastInput * Time.deltaTime, boosting);
    }

    void OnMove(InputValue value)
    {
        lastInput = value.Get<Vector2>();
        Debug.Log("Ship -- OnMove");
    }

    void OnBoost(InputValue value)
    {
        Debug.Log("Ship -- OnBoost");
        boosting = value.Get<float>() <= 0.5f ? false : true;
    }

    void OnJobSwap()
    {
        Debug.Log("Ship -- OnJobSwap");
    }

    void OnMapToggle()
    {
        Debug.Log("Ship -- OnMapToggle");
    }

    void OnRotateLeft()
    {
        Debug.Log("Ship -- OnRotateLeft");
    }

    void OnRotateRight()
    {
        Debug.Log("Ship -- OnRotateRight");
    }

    void OnBreak()
    {
        Debug.Log("Ship -- OnBreak");
    }
}