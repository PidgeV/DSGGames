using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

/// <summary>
/// Controls the ship based on the controller input.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class ShipController : Controller
{
    [SerializeField]
    private ShipStats stats;

    [SerializeField]
    private bool drawDebug;

    private Camera cam;
    private Rigidbody rigid;

    private float thrustSpeed;
    private float strafeSpeed;
    private float rotationSpeed;
    private float rollSpeed;

    private float boostMeter;

    private Vector2 strafeDirection;
    private Vector2 rotateDirection;
    private int rollDirection;

    private Vector3 finalStrafeVelocity;
    private Vector3 finalThrustVelocity;
    private Vector3 finalRotation;
    private Vector3 finalRollRotation;

    private bool strafing;
    private bool rotating;
    private bool boosting;

    private void Awake()
    {
        // Sets camera to top half screen
        cam = GetComponent<PlayerInput>().camera;
        cam.rect = new Rect(0, 0.5f, 1.0f, 0.5f);

        rigid = GetComponent<Rigidbody>();

        // Sets the center of mass
        //rigid.centerOfMass = transform.Find("Center of Mass").position;

        boostMeter = stats.maxBoostGauge;
    }

    void Update()
    {
        // Right Stick
        #region Ship Rotation

        // Used for acceleration and deacceleration
        if (rotating)
            rotationSpeed = Mathf.Clamp(rotationSpeed + stats.shipRotAcceleration * Time.deltaTime, 0, stats.rotationSpeed);
        else
            rotationSpeed = Mathf.Clamp(rotationSpeed - stats.shipRotDeceleration * Time.deltaTime, 0, stats.rotationSpeed);

        finalRotation = rotateDirection * rotationSpeed;

        #endregion

        // Bumpers
        #region Roll Rotation

        // Used for acceleration and deacceleration
        if (ControllerInput.LeftBumper)
        {
            rollDirection = 1;
            rollSpeed = Mathf.Clamp(rollSpeed + stats.shipRotAcceleration * Time.deltaTime, 0, stats.rotationSpeed);
        }
        else if (ControllerInput.RightBumper)
        {
            rollDirection = -1;
            rollSpeed = Mathf.Clamp(rollSpeed + stats.shipRotAcceleration * Time.deltaTime, 0, stats.rotationSpeed);
        }
        else
        {
            rollSpeed = Mathf.Clamp(rollSpeed - stats.shipRotDeceleration * Time.deltaTime, 0, stats.rotationSpeed);
        }

        finalRollRotation = new Vector3(0, 0, rollDirection) * rollSpeed;

        #endregion

        // Triggers
        #region Thrust Velocity

        // Used for boosting speed and normal thruster speed
        if (boosting)
        {
            thrustSpeed = Mathf.Clamp(thrustSpeed + stats.shipAcceleration * 1.5f * Time.deltaTime, stats.minThrustSpeed, stats.boostSpeed);

            boostMeter = Mathf.Clamp(boostMeter - stats.boostGaugeConsumeAmount * Time.deltaTime, 0, stats.maxBoostGauge);

            if (boostMeter <= 0)
            {
                boosting = false;
            }
        }
        else if (thrustSpeed > stats.maxThrustSpeed)
        {
            thrustSpeed = Mathf.Clamp(thrustSpeed - stats.shipDeceleration * Time.deltaTime, stats.maxThrustSpeed, stats.boostSpeed);
        }
        else
            thrustSpeed = Mathf.Clamp(thrustSpeed + (ControllerInput.RightTrigger * stats.shipAcceleration - ControllerInput.LeftTrigger * stats.shipDeceleration) * Time.deltaTime, stats.minThrustSpeed, stats.maxThrustSpeed);

        if (boostMeter < stats.maxBoostGauge)
            boostMeter = Mathf.Clamp(boostMeter + stats.boostGaugeRechargeAmount * Time.deltaTime, 0, stats.maxBoostGauge);

        finalThrustVelocity = transform.forward * thrustSpeed;

        #endregion

        // Left Stick
        #region Strafe Velocity

        // Used for acceleration and deacceleration
        if (strafing)
            strafeSpeed = Mathf.Clamp(strafeSpeed + stats.shipAcceleration * Time.deltaTime, 0, stats.strafeSpeed);
        else
            strafeSpeed = Mathf.Clamp(strafeSpeed - stats.shipDeceleration * Time.deltaTime, 0, stats.strafeSpeed);

        finalStrafeVelocity = strafeDirection * strafeSpeed;

        #endregion
    }

    private void FixedUpdate()
    {
        // Apply physics to rigidbody
        Quaternion rollRotation = Quaternion.Euler(finalRollRotation);
        Quaternion rotateRotation = Quaternion.Euler(finalRotation);

        rigid.rotation *= rollRotation * rotateRotation;

        rigid.AddForce(finalThrustVelocity, ForceMode.VelocityChange);
        rigid.AddRelativeForce(finalStrafeVelocity, ForceMode.VelocityChange);
    }

    private void OnGUI()
    {
        if (drawDebug)
        {
            GUI.Label(new Rect(5, 5, 500, 200), "Left Stick: " + ControllerInput.LeftStick +
                                                "\nRight Stick: " + ControllerInput.RightStick +
                                                "\nLeft Trigger: " + ControllerInput.LeftTrigger +
                                                "\nRight Trigger: " + ControllerInput.RightTrigger);

            GUI.Label(new Rect(Screen.width - 205, 5, 200, 200), "Thrust Speed: " + thrustSpeed +
                                                "\nStrafe Speed: " + strafeSpeed +
                                                "\nRotation Speed: " + rotationSpeed +
                                                "\nRoll Speed: " + rollSpeed +
                                                "\nBoost Meter: " + (boostMeter / stats.maxBoostGauge) * 100 + "%" +
                                                "\nFinal Rotation: " + finalRotation +
                                                "\nFinal Roll Rotation: " + finalRollRotation);
        }
    }

    /// <summary>
    /// Ship strafes in the the direction of the left stick
    /// </summary>
    /// <param name="input"></param>
    public override void OnLeftStick(InputValue input)
    {
        base.OnLeftStick(input);

        if (ControllerInput.LeftStick.sqrMagnitude >= 1)
        {
            strafeDirection = ControllerInput.LeftStick;
            strafing = true;
        }
        else
        {
            strafing = false;
        }
    }

    /// <summary>
    /// Ship rotates in the direction of the right stick
    /// </summary>
    /// <param name="input"></param>
    public override void OnRightStick(InputValue input)
    {
        base.OnRightStick(input);

        if (ControllerInput.RightStick != Vector2.zero)
        {
            rotateDirection = new Vector2(-ControllerInput.RightStick.y, ControllerInput.RightStick.x);
            rotating = true;
        }
        else
        {
            rotating = false;
        }
    }

    /// <summary>
    /// Boosts ship
    /// </summary>
    /// <param name="input"></param>
    public override void OnA(InputValue input)
    {
        base.OnA(input);

        if (!boosting && boostMeter == stats.maxBoostGauge)
        {
            boosting = true;
        }
    }
}
