using SNSSTypes;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class ShipController : MonoBehaviour
{
	public Camera PilotCamera;
	public Camera GunnerCamera;

	[HideInInspector] public Player player1;
	[HideInInspector] public Player player2;

	public ParticleSystem warpEffect1;
	public ParticleSystem warpEffect2;

	public Transform ShipModel;

	[SerializeField] private Transform gunnerPivot;
	[SerializeField] private Transform gunnerObject;
	[SerializeField] private Transform gunnerslockUI;

    ShieldProjector shieldProjector;

	public bool invertedControls;
	public bool unlimitedBoost;
	public bool lockOn;

	public ShipBehaviour myBehaviour;

	[Header("Stats")]
	public PlayerStats myStats;

	private PlayerHUDHandler _playerHUD;
	private WeaponsSystem _weaponsSystem;

	[SerializeField] private ShootingSoundController shootingSoundController;

	private HealthAndShields shipHP;
	private Rigidbody rigidbody;


	// Who are we locked onto
	private GameObject lockOnTarget;

	#region Ship Properties
	private int numberOfPlayers;

	private float thrustSpeed;
	private float strafeSpeed;
	private float rotationSpeed;
	private float rollDirection;
	private float rollInput;
	private float rollSpeed;
	private float spinSpeed;
	private float turnMultiplayer;

	private Vector2 strafeDirection;
	private Vector2 rotateDirection;
	private Vector2 gunnerCenter;

	private Vector3 finalStrafeVelocity;
	private Vector3 finalThrustVelocity;
	private Vector3 finalRollRotation;
	private Vector3 finalRotation;

	private bool strafing;
	private bool rotating;
	private bool boosting;
	private bool roleSwap;
	private bool stopThrust;
	private bool shooting_Gunner;
	private bool shooting_Pilot;
	private bool slowCamera;

	private float boostGauge;
	private float shotTimer;

	private float gunRotationSpeed = 200f;

	private Vector2 gunVelocity;
	private Vector2 gunRotation;

	private Quaternion gunnerAim;
	#endregion

	#region Properties
	public int NumberOfPlayers { get { return numberOfPlayers; } set { numberOfPlayers = value; } }

	public bool Boosting { get { return boosting; } set { boosting = value; } }
	public bool Strafing { get { return strafing; } set { strafing = value; } }
	public bool Rotating { get { return rotating; } set { rotating = value; } }
	public bool RoleSwap { get { return roleSwap; } set { roleSwap = value; } }
	public bool StopThrust { get { return stopThrust; } set { stopThrust = value; } }

	public float RollInput { get { return rollInput; } set { rollInput = value; } }
	public float BoostGauge { get { return boostGauge; }  set { boostGauge = value; } }

	public float TurnMultiplayer { get { return turnMultiplayer; }  set { turnMultiplayer = value; } }
	public float GunRotationSpeed { get { return gunRotationSpeed; } set { gunRotationSpeed = value; } }

	public Vector2 StrafeDirection { get { return strafeDirection; } set { strafeDirection = value; } }
	public Vector2 RotateDirection { get { return rotateDirection; } set { rotateDirection = value; } }

	public Vector3 GunVelocity { get { return gunVelocity; }  set { gunVelocity = value; } }
	#endregion

	// Unity Methods and Events
	#region Unity Events and Methods

	/// <summary>
	/// Initialize and Get dependencies
	/// </summary>
	private void Awake()
	{
		foreach (Player player in GameObject.FindObjectsOfType<Player>()) {
			player.FindShip();
		}

		// Set the cameras size and positions
		GunnerCamera.rect = new Rect(0, 0.0f, 1.0f, 0.5f);
		PilotCamera.rect = new Rect(0, 0.5f, 1.0f, 0.5f);

		gunnerCenter = gunnerslockUI.transform.position;

		// Set the max values of our Boost Gauge
		boostGauge = myStats.maxBoostGauge;

		// Get Components
		rigidbody = gameObject.GetComponent<Rigidbody>();

		_playerHUD = gameObject.GetComponent<PlayerHUDHandler>();
		_weaponsSystem = gameObject.GetComponent<WeaponsSystem>();

		// Get references
		if (!shipHP) TryGetComponent(out shipHP);

        shieldProjector = GetComponentInChildren<ShieldProjector>();
    }

	/// <summary>
	/// Used for all the non physics calculations
	/// </summary>
	private void Update()
	{
		if (MenuManager.Instance.Sleeping) return;

		UpdateShipValues();
		
		UpdateShipModel(rotateDirection);

		UpdateTurnMultiplier();
	}

	/// <summary>
	/// FixedUpdate is used for physic calculations
	/// </summary>
	private void FixedUpdate()
	{
		if (MenuManager.Instance.Sleeping) return;
		UpdateShipPhysics();
	}

    /// <summary>
    /// LateUpdate is used to set the gunners camera, the reason for this is
    /// because we need to know where the ship is after physics are applied
    /// </summary>
    private void LateUpdate()
	{
		if (MenuManager.Instance.Sleeping) return;
		UpdateCamera();
	}

	#endregion

	// Update transformations for the gun or the ship
	#region Ship Movement Methods

	/// <summary> Change the roles of the players </summary>
	public void SwapRoles()
	{
		if (player1) player1.SwapRole();
		if (player2) player2.SwapRole();

		ResetShip();
	}

	/// <summary>
	/// Use INPUT and update the camera
	/// </summary>
	public void UpdateCamera()
	{
		// Where the camera SHOULD be relative to the ship model
		Vector3 shipPos = ShipModel.transform.position;

		Vector3 yPos = -6 * ShipModel.transform.up;
		Vector3 zPos = -1 * ShipModel.transform.forward;

		gunnerPivot.position = Vector3.Lerp(gunnerPivot.position, shipPos + yPos + zPos, 0.25f);

		gunnerPivot.rotation = Quaternion.identity;
		gunnerObject.Rotate(new Vector2(-gunVelocity.y, gunVelocity.x) * Time.deltaTime * (slowCamera ? 0.2f : 1f));

		// If we don't have a target currently locking
		if (lockOnTarget == null)
		{
			CheckForTarget();
		}

		// (BLOCKER) If we cant Lock on
		if (lockOn == false) return;

		if (lockOnTarget != null)
		{
			Vector2 newPoint = GunnerCamera.WorldToScreenPoint(lockOnTarget.transform.position);
			float unlockDist = 60;

			if (Vector2.Distance(gunnerCenter, newPoint) < unlockDist)
			{
				gunnerslockUI.transform.position = newPoint;
			}
			else
			{
				gunnerslockUI.transform.position = gunnerCenter;
				lockOnTarget = null;
			}
		}
	}

	/// <summary>
	/// Raycast forward and see if the gunners hovering an enemy
	/// </summary>
	public void CheckForTarget()
	{
		// (BLOCKER) If we cant Lock on
		if (lockOn == false) return;

		RaycastHit hit;

		Vector3 origin = GunnerCamera.transform.position;
		Vector3 direction = GunnerCamera.transform.forward;

		if (Physics.Raycast(origin, direction, out hit, Mathf.Infinity, LayerMask.GetMask("Enemies")))
		{
			Debug.Log("Locked ON!", hit.collider.gameObject);
			lockOnTarget = hit.collider.gameObject;
		}
	}

	/// <summary>
	/// Use INPUT and update the ships target values, not the actual rigidbody
	/// </summary>
	public void UpdateShipValues()
	{
		// Steer Ship
		#region Steer Ship


		if (rotating)
		{
			rotationSpeed = Mathf.Clamp(rotationSpeed + myStats.shipRotAcceleration , 0, myStats.rotationSpeed);
		}
		else
		{
			rotationSpeed = Mathf.Clamp(rotationSpeed - myStats.shipRotAcceleration , 0, myStats.rotationSpeed);
		}

		finalRotation = rotateDirection * rotationSpeed;
		#endregion

		// Rotate Ship
		#region  Rotate Ship

		if (rollInput > 0)
		{
			rollDirection = 1;
			rollSpeed = Mathf.Clamp(rollSpeed + myStats.shipRotAcceleration, 0, myStats.rotationSpeed);
		}
		else if (rollInput < 0)
		{
			rollDirection = -1;
			rollSpeed = Mathf.Clamp(rollSpeed + myStats.shipRotAcceleration, 0, myStats.rotationSpeed);
		}
		else
		{
			rollSpeed = Mathf.Clamp(rollSpeed - myStats.shipRotDeceleration, 0, myStats.rotationSpeed);
		}

		finalRollRotation = new Vector3(0, 0, rollDirection) * rollSpeed;
		#endregion

		// Strafe Ship
		#region Strafe Ship

		if (strafing)
		{
			strafeSpeed = Mathf.Clamp(strafeSpeed + myStats.shipAcceleration * 2f, 0, myStats.strafeSpeed);
		}
		else
		{
			strafeSpeed = Mathf.Clamp(strafeSpeed - myStats.shipDeceleration, 0, myStats.strafeSpeed);
		}
		finalStrafeVelocity = strafeDirection * strafeSpeed;
		#endregion

		// Ship Thrust
		#region Ship Thrust

		if (stopThrust)
		{
			thrustSpeed = Mathf.Clamp(thrustSpeed - myStats.shipDeceleration, 0, myStats.shipSpeed);
		}
		// If were boosting
		else if (boosting)
		{
			// Increase our speed when boosting
			if (rigidbody.velocity.magnitude < myStats.boostSpeed)
				thrustSpeed = Mathf.Clamp(thrustSpeed + (myStats.shipSpeed * 2.4f * Time.deltaTime), myStats.shipSpeed, myStats.boostSpeed);


			// Reduce the boost gauge
			boostGauge = Mathf.Clamp(boostGauge - myStats.boostGaugeConsumeAmount * Time.deltaTime, 0, myStats.maxBoostGauge);
			_playerHUD._slider_Boost.value = 1 / myStats.maxBoostGauge * boostGauge;

			// Set the color of the boost slider
			_playerHUD._boostImage.color = Color.Lerp(Color.red, Color.yellow, 1 / myStats.maxBoostGauge * boostGauge);

			// Turn off boosting
			if (boostGauge <= 0 && unlimitedBoost == false)
			{
				boosting = false;
			}
		}
		// Else if we are NOT boosting and our thrustSpeed is over our maxThrustSpeed
		// We need to smooth the transition from boosting to not boosting
		else if (thrustSpeed > myStats.shipSpeed)
		{
			// Clamp our thrust Speed to our max thrust speed
			thrustSpeed = Mathf.Clamp(thrustSpeed - myStats.shipDeceleration, myStats.shipSpeed, myStats.boostSpeed);
		}
		// Else were not boosting so we increase our ships normal speed
		else
		{
			thrustSpeed = myStats.shipSpeed;
		}

		finalThrustVelocity = transform.forward * thrustSpeed;
		#endregion
	}

	/// <summary>
	/// Update the ships movement
	/// </summary>
	public void UpdateShipPhysics()
	{
		// Apply physics to rigidbody
		Quaternion rollRotation = Quaternion.Euler(finalRollRotation);
		Quaternion rotateRotation = Quaternion.Euler(finalRotation);

		rigidbody.rotation *= rollRotation * rotateRotation;

		rigidbody.AddForce(finalThrustVelocity, ForceMode.VelocityChange);
		rigidbody.AddRelativeForce(finalStrafeVelocity, ForceMode.VelocityChange);
	}

	/// <summary>
	/// Move the ships model so it looks more like your controlling the ship
	/// </summary>
	public void UpdateShipModel(Vector3 velocity)
	{
		// Rotation
		Quaternion currentRot = ShipModel.transform.localRotation;
		Quaternion targetRot = Quaternion.Euler
			(
				 velocity.x * myBehaviour.xRot,
				 velocity.z * myBehaviour.yRot,
				-velocity.y * myBehaviour.zRot
			);

		ShipModel.transform.localRotation = Quaternion.Slerp(currentRot, targetRot, myBehaviour.RotSpeed);

		// Translation
		Vector3 currentPos = ShipModel.transform.localPosition;
		Vector3 targetPos = new Vector3(-velocity.y, velocity.x, 0) * myBehaviour.moveScale;

		if (targetPos.y < 0) {
			targetPos.y *= 0.4f;
		}

		ShipModel.transform.localPosition = Vector3.Lerp(currentPos, targetPos, myBehaviour.moveSpeed);

		// Camera
		Vector3 targetCameraPos = boosting ? myBehaviour.boostPos : myBehaviour.normalPos;

		PilotCamera.transform.localPosition = Vector3.Slerp(PilotCamera.transform.localPosition, targetCameraPos, myBehaviour.cameraSpeed);
	}

	#endregion

	private void UpdateTurnMultiplier()
	{
		if (boosting)
		{
			turnMultiplayer = Mathf.Lerp(turnMultiplayer, 0.4f, 0.03f);
		}
		else
		{
			turnMultiplayer = Mathf.Lerp(turnMultiplayer, 1f, 0.03f);
		}
	}

	private void ResetShip()
	{
		gunVelocity = Vector3.zero;
	}

	public void SwapWeapon(Vector2 Input)
	{
		_weaponsSystem.SwapWeapon(Input);
	}

	public void ShootGun(bool pressed)
	{
		_weaponsSystem.ShootGun(pressed);
	}

	public void ShootShip(bool pressed)
	{
		_weaponsSystem.ShipShooting = pressed;
	}
	
	public void SlowCamera(bool pressed)
	{
		slowCamera = pressed;
	}

	// Editor values
	[HideInInspector] public bool showBehaviour = false;
	[HideInInspector] public bool showStats = false;
	[HideInInspector] public bool useDefaultEditor = false;
}
