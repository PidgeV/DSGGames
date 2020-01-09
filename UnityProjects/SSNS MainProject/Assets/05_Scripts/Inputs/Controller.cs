using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Controller reads inputs from a pluged in controller / player
/// </summary>
public abstract class Controller : MonoBehaviour
{
	/// <summary>buttons is the REAL values that represents this contrller</summary>
	public ControllerActions ControllerInput = new ControllerActions();

	/// <summary>When the left joystick is pressed</summary>
	public virtual void OnLeftStick(InputValue input) { ControllerInput.LeftStick = input.Get<Vector2>(); }
	/// <summary>When the right joystick is pressed</summary>
	public virtual void OnRightStick(InputValue input) { ControllerInput.RightStick = input.Get<Vector2>(); }

	/// <summary>When the DPad is pressed</summary>
	public virtual void OnDPad(InputValue input) { ControllerInput.DPad = input.Get<Vector2>(); }

	/// <summary>When the left trigger is pressed</summary>
	public virtual void OnLeftTrigger(InputValue input) { ControllerInput.LeftTrigger = input.Get<float>(); }
	/// <summary>When the right trigger is pressed</summary>
	public virtual void OnRightTrigger(InputValue input) { ControllerInput.RightTrigger = input.Get<float>(); }
	/// <summary>When the left Bumper is pressed</summary>
	public virtual void OnLeftBumper(InputValue input) { ControllerInput.LeftBumper = input.Get<float>(); }
	/// <summary>When the right Bumper is pressed</summary>
	public virtual void OnRightBumper(InputValue input) { ControllerInput.RightBumper = input.Get<float>(); }

	/// <summary>When the a button is pressed</summary>
	public virtual void OnA(InputValue input) { ControllerInput.A = input.Get<float>(); }
	/// <summary>When the b button is pressed</summary>
	public virtual void OnB(InputValue input) { ControllerInput.B = input.Get<float>(); }
	/// <summary>When the y button is pressed</summary>
	public virtual void OnY(InputValue input) { ControllerInput.Y = input.Get<float>(); }
	/// <summary>When the x button is pressed</summary>
	public virtual void OnX(InputValue input) { ControllerInput.X = input.Get<float>(); }
}

/// <summary>
/// ControllerActions holds values for each button on a controller
/// </summary>
public class ControllerActions
{
	/// <summary>XBox Controller's [ Left Joystick ]</summary>
	public Vector2 LeftStick = new Vector2(0, 0);
	/// <summary>XBox Controller's [ Right Joystick ]</summary>
	public Vector2 RightStick = new Vector2(0, 0);

	/// <summary>XBox Controller's [ Directional Pad ]</summary>
	public Vector2 DPad = new Vector2(0, 0);

	/// <summary>XBox Controller's [ Left Trigger ]</summary>
	public float LeftTrigger = 0.0f;
	/// <summary>XBox Controller's [ Right Trigger ]</summary>
	public float RightTrigger = 0.0f;
	/// <summary>XBox Controller's [ Left Bumper ]</summary>
	public float LeftBumper = 0.0f;
	/// <summary>XBox Controller's [ Right Bumper ]</summary>
	public float RightBumper = 0.0f;

	/// <summary>XBox Controller's [ A Button ]</summary>
	public float A = 0.0f;
	/// <summary>XBox Controller's [ B Button ]</summary>
	public float B = 0.0f;
	/// <summary>XBox Controller's [ Y Button ]</summary>
	public float Y = 0.0f;
	/// <summary>XBox Controller's [ X Button ]</summary>
	public float X = 0.0f;
}
