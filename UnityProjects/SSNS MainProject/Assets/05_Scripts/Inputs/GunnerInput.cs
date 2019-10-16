﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// NOTE -- This script shoud not be on anything by default
//           It is given to an Input Object after the player selects their role

public class GunnerInput : MonoBehaviour
{
    public TurretController turretController;

    Vector3 lastInput = new Vector3();

    private void Start()
    {
        turretController = FindObjectOfType<TurretController>();
    }

    private void Update()
    {
        turretController.Move(lastInput * Time.deltaTime);
    }

    void OnMove(InputValue value)
    {
        lastInput = value.Get<Vector2>();
        Debug.Log("Gunner -- OnMove");
    }

    void OnJobSwap()
    {
        Debug.Log("Gunner -- OnJobSwap");
    }

    void OnMapToggle()
    {
        Debug.Log("Gunner -- OnMapToggle");
    }

    void OnShoot()
    {
        Debug.Log("Gunner -- OnShoot");
    }

    void OnChangeWeapons()
    {
        Debug.Log("Gunner -- OnChangeWeapons");
    }
}