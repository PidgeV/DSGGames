﻿using SNSSTypes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// TODO -- Menu Navigation
// TODO -- Player Score
// TODO -- Leaving Play Area
// TODO -- Press A to Join
// TODO -- Mouse controls
// TODO -- Less movemement flexibility while boosting



[RequireComponent(typeof(Rigidbody))]
public class ShipController : MonoBehaviour
{
	[Header("Cameras")]
	[SerializeField] private Camera pilotCamera;
	[SerializeField] private Camera gunnerCamera;

	[Header("Dependencies")]
	[SerializeField] private Transform shipModel;
	[SerializeField] private Transform gunnerPivot;
	[SerializeField] private Transform gunnerObject;
	[SerializeField] private Transform barrelL;
	[SerializeField] private Transform barrelR;
	[SerializeField] private Transform gunnerslockUI;
	[SerializeField] private GunController gunController;
    ShieldProjector shieldProjector;

    [Header("Control Options")]
	[SerializeField] private bool invertedControls;
	[SerializeField] private bool unlimitedBoost;
	[SerializeField] private bool limitRotation;
	[SerializeField] private bool lockOn;

	[Header("Behaviour")]
	public ShipBehaviour myBehaviour;

	[Header("Stats")]
	public ShipStats myStats;

	[Header("Weapon")]
	[SerializeField] private WeaponType startingWeapon;
	[SerializeField] private LaserBehaviour laser;
	[SerializeField] private ShotInfo[] shots;
    private ChargedShotBehaviour chargedShot;

    // The ammo we have for each weapon
    public AmmoCounter ammoCount;

	// What weapon are we currently using
	[HideInInspector] public WeaponType currentWeapon;

	// A Reference to the player controlling this ship if needed
	[HideInInspector] public Player player1;
	[HideInInspector] public Player player2;


	[Header("Audio")]
	[SerializeField] private ShootingSoundController shootingSoundController;

	private Slider slider_Health;
	private Slider slider_Shield;
	private Slider slider_Boost;

	private Image healthImage;
	private Image shieldImage;
	private Image boostImage;

	private HealthAndShields shipHP;
	private Rigidbody rigidbody;

	// The data about the current weapon
	private ShotInfo currentShotInfo;

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

	private float boostGauge;
	private float shotTimer;

	private float gunRotationSpeed = 200f;

	private Vector2 gunVelocity;
	private Vector2 gunRotation;

	private Quaternion gunnerAim;
	#endregion

	// Editor values
	[HideInInspector] public bool showBehaviour = false;
	[HideInInspector] public bool showStats = false;

	// Proporties
	public bool StopThrust { set { stopThrust = value; } }


	// Unity Methods and Events
	#region Unity Events and Methods

	/// <summary>
	/// Initialize and Get dependencies
	/// </summary>
	private void Awake()
	{
		// Set the cameras size and positions
		gunnerCamera.rect = new Rect(0, 0.0f, 1.0f, 0.5f);
		pilotCamera.rect = new Rect(0, 0.5f, 1.0f, 0.5f);

		gunnerCenter = gunnerslockUI.transform.position;

		// Set the max values of our boost Gauge
		boostGauge = myStats.maxBoostGauge;

		// Get components
		rigidbody = gameObject.GetComponent<Rigidbody>();

		slider_Health = GameObject.Find("[Slider] Health").GetComponent<Slider>();
		slider_Shield = GameObject.Find("[Slider] Shield").GetComponent<Slider>();
		slider_Boost = GameObject.Find("[Slider] Boost").GetComponent<Slider>();

		// Get the images for the sliders
		boostImage = slider_Boost.gameObject.GetComponentInChildren<Image>();
		healthImage = slider_Health.gameObject.GetComponentInChildren<Image>();
		shieldImage = slider_Shield.gameObject.GetComponentInChildren<Image>();

		// Get references
		if (!shipHP) TryGetComponent(out shipHP);

		currentWeapon = startingWeapon;

		shotTimer = Mathf.Infinity;

		foreach (ShotInfo s in shots)
		{
			if (s.weapon == currentWeapon)
			{
				currentShotInfo = s;
			}
		}

        shieldProjector = GetComponentInChildren<ShieldProjector>();

    }

	/// <summary>
	/// Used for all the non physics calculations
	/// </summary>
	private void Update()
    {
        #region Shooting Logic

        shotTimer += Time.deltaTime;
        if (currentWeapon != currentShotInfo.weapon)
        {
            foreach (ShotInfo s in shots)
            {
                if (s.weapon == currentWeapon)
                {
                    currentShotInfo = s;
                }
            }
        }

		#endregion

		// Update the ships movement values
		UpdateShipValues();

		// Move the ship model
		UpdateShipModel(rotateDirection);

		// Update sliders
		UpdateBoostGauge();
		UpdateHealthAndShields();

		if (boosting)
		{
			turnMultiplayer = Mathf.Lerp(turnMultiplayer, 0.4f, 0.03f);
		}
		else
		{
			turnMultiplayer = Mathf.Lerp(turnMultiplayer, 1f, 0.03f);
		}

		#region TODO -- Check if does anything
		// Shooting
		if (shooting_Gunner)
        {
            Shoot();
        }
        else
        {
            laser.fadeIn = false;

            if (chargedShot)
            {
                chargedShot.HasShot = true;
                chargedShot = null;
            }
        }

        if (shooting_Pilot)
        {
            ShipShoot();
        }
		#endregion
	}

	/// <summary>
	/// FixedUpdate is used for physic calculations
	/// </summary>
	private void FixedUpdate()
    {
		// Update the ships PHYSICS and rotation
		UpdateShipPhysics();
	}

    /// <summary>
    /// LateUpdate is used to set the gunners camera, the reason for this is
    /// because we need to know where the ship is after physics are applied
    /// </summary>
    private void LateUpdate()
    {
		UpdateCamera();
	}

	/// <summary>
	/// Clean up 
	/// </summary>
	private void OnDestroy()
	{
		//if (player1) Destroy(player1.gameObject);

		//if (player2) Destroy(player2.gameObject);
	}

	#endregion

	// Update transformations for the gun or the ship
	#region Ship Movement Methods

	/// <summary>
	/// Add a player to the ship 
	/// </summary>
	public void JoinShip(Player newPlayer)
	{
		// Check if we are the first player to join
		if (player1)
		{
			// If we have a pilot

			// Make the new player a gunner
			if (player1.myRole == PlayerRole.Pilot)
			{
				player2 = newPlayer;
				player2.myRole = PlayerRole.Gunner;
			}
			else
			{
				player2 = newPlayer;
				player2.myRole = PlayerRole.Pilot;
			}
		}
		else
		{
			// If we dont have a pilot

			// Make the new player a pilot
			player1 = newPlayer;
			player1.myRole = PlayerRole.Pilot;
		}

		// Increment the number of players
		numberOfPlayers++;
	}

	/// <summary> 
	/// Use INPUT and update the camera 
	/// </summary>
	public void UpdateCamera()
	{
		gunnerPivot.rotation = Quaternion.identity;
		gunnerObject.Rotate(new Vector2(-gunVelocity.y, gunVelocity.x) * Time.deltaTime);

		// POSITION
		// Where the camera SHOULD be relative to the ship model
		Vector3 shipPos = shipModel.transform.position;

		Vector3 yPos = -6 * shipModel.transform.up;
		Vector3 zPos = -1 * shipModel.transform.forward;

		gunnerPivot.position = Vector3.Lerp(gunnerPivot.position, shipPos + yPos + zPos, 0.25f);

		// If we don't have a target currently locking
		if (lockOnTarget == null)
		{
			CheckForTarget();
		}


		// (BLOCKER) If we cant Lock on
		if (lockOn == false) return;

		if (lockOnTarget != null)
		{
			Vector2 newPoint = gunnerCamera.WorldToScreenPoint(lockOnTarget.transform.position);
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

		Vector3 origin = gunnerCamera.transform.position;
		Vector3 direction = gunnerCamera.transform.forward;

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
			thrustSpeed = Mathf.Clamp(thrustSpeed - myStats.shipDeceleration, 0, myStats.thrustSpeed);
		}
		// If were boosting
		else if (boosting)
		{
			// Increase our speed when boosting
			if (rigidbody.velocity.magnitude < myStats.boostSpeed)
				thrustSpeed = Mathf.Clamp(thrustSpeed + (myStats.thrustSpeed * 2.4f * Time.deltaTime), myStats.thrustSpeed, myStats.boostSpeed);


			// Reduce the boost gauge
			boostGauge = Mathf.Clamp(boostGauge - myStats.boostGaugeConsumeAmount * Time.deltaTime, 0, myStats.maxBoostGauge);
			slider_Boost.value = 1 / myStats.maxBoostGauge * boostGauge;

			// Set the color of the boost slider
			boostImage.color = Color.Lerp(Color.red, Color.yellow, 1 / myStats.maxBoostGauge * boostGauge);

			// Turn off boosting
			if (boostGauge <= 0 && unlimitedBoost == false)
			{
				boosting = false;
			}
		}
		// Else if we are NOT boosting and our thrustSpeed is over our maxThrustSpeed
		// We need to smooth the transition from boosting to not boosting
		else if (thrustSpeed > myStats.thrustSpeed)
		{
			// Clamp our thrust Speed to our max thrust speed
			thrustSpeed = Mathf.Clamp(thrustSpeed - myStats.shipDeceleration, myStats.thrustSpeed, myStats.boostSpeed);
		}
		// Else were not boosting so we increase our ships normal speed
		else
		{
			thrustSpeed = myStats.thrustSpeed;
		}

		finalThrustVelocity = transform.forward * thrustSpeed;
		#endregion
	}

	/// <summary> 
	/// Update the ships PHYSICS
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
		Quaternion currentRot = shipModel.transform.localRotation;
		Quaternion targetRot = Quaternion.Euler
			(
				 velocity.x * myBehaviour.xRot,
				 velocity.z * myBehaviour.yRot,
				-velocity.y * myBehaviour.zRot
			);

		shipModel.transform.localRotation = Quaternion.Slerp(currentRot, targetRot, myBehaviour.RotSpeed);

		// Translation
		Vector3 currentPos = shipModel.transform.localPosition;
		Vector3 targetPos = new Vector3(-velocity.y, velocity.x, 0) * myBehaviour.moveScale;

		if (targetPos.y < 0) {
			targetPos.y *= 0.4f;
		}

		shipModel.transform.localPosition = Vector3.Lerp(currentPos, targetPos, myBehaviour.moveSpeed);

		// Camera
		Vector3 targetCameraPos = boosting ? myBehaviour.boostPos : myBehaviour.normalPos;

		pilotCamera.transform.localPosition = Vector3.Slerp(pilotCamera.transform.localPosition, targetCameraPos, myBehaviour.cameraSpeed);
	}

	#endregion

	// Shooting
	#region Projectile Methods

	private void SpawnShot()
	{
		Quaternion rot = Quaternion.LookRotation(barrelL.transform.forward);
		GameObject shot = Instantiate(currentShotInfo.gameObject, barrelL.position, rot);

		shootingSoundController.PlayShot(currentWeapon);

        if (shot.TryGetComponent(out ShotThing st))
        {
            st.whoSent = ShotThing.shotFrom.Player;
        }

        shieldProjector.IgnoreCollider(shot.GetComponent<Collider>());
	}

	public void Shoot()
	{
		Ray ray = new Ray(gunnerCamera.transform.position, gunnerCamera.transform.forward);
		LayerMask layer = LayerMask.GetMask("Player");

		//If not hitting player collider
		if (!Physics.Raycast(ray, 15f, layer))
		{
			if (currentWeapon == WeaponType.Laser)
			{
                LaserOn();
			}
			else if (currentWeapon == WeaponType.Charged && shotTimer > currentShotInfo.FireRate)
			{
                ChargeShot();
			}
			else if (shotTimer > currentShotInfo.FireRate)
			{
				laser.fadeIn = false;
				shotTimer = 0;

				if (ammoCount.HasAmmo(currentWeapon))
				{
					ammoCount.Take1Ammo(currentWeapon);
					SpawnShot();
				}
			}
		}
        else
        {
            laser.fadeIn = false;
        }

        //Spawn shot effect
        if(TryGetComponent(out ShotEffectSpawner effectSpawn))
        {
            effectSpawn.SpawnEffect(currentWeapon);
        }
	}

    void LaserOn()
    {
        laser.fadeIn = true;
        //LaserShot: Long, straight beam, near instant, aoe from origin
        //Raycast instead of instantiate for instant movement
        ammoCount.Take1Ammo(currentWeapon);

        LayerMask enemyLayer = LayerMask.GetMask("Enemies");
        enemyLayer += LayerMask.GetMask("Swarm");
        RaycastHit[] hits = Physics.SphereCastAll(barrelL.transform.position, laser.radius, barrelL.transform.forward.normalized, laser.Length, enemyLayer);

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.TryGetComponent(out HealthAndShields hp))
            {
                hp.TakeDamage(laser.Damage / 3, laser.Damage);
            }
        }
    }

    void ChargeShot()
    {
        laser.fadeIn = false;
        shotTimer = 0;
        //Spawn then parent to the gunner. Store gameobject to check in update.
        if (!chargedShot)
        {
            Quaternion rot = Quaternion.LookRotation(barrelL.transform.forward);
            chargedShot = Instantiate(currentShotInfo.gameObject, barrelL.position, rot).GetComponent<ChargedShotBehaviour>();
            chargedShot.transform.parent = barrelL.transform;

            shieldProjector.IgnoreCollider(chargedShot.GetComponent<Collider>());
        }
    }

	public void ShipShoot()
	{
		if (shotTimer > currentShotInfo.FireRate)
		{
			shotTimer = 0;
			GameObject shot = Instantiate(currentShotInfo.gameObject, transform.position + transform.forward * 15, shipModel.transform.rotation);
		}
	}

	#endregion

	// Called by the player script
	#region Input Methods

	/// <summary> Move the gunners camera </summary>
	public void AimGun(Vector2 velocity)
    {
		gunVelocity = new Vector2(velocity.x, velocity.y) * gunRotationSpeed;
    }

	/// <summary> Steer the ship </summary>
	float turnMultiplayer = 1;
    public void SteerShip(Vector2 velocity)
    {
        // Check if we are steering
        if (velocity == Vector2.zero)
        {
            rotateDirection = Vector2.zero;
            rotating = false;
        }
        else
        {
            // Check if we want inverted controls
            if (invertedControls == true)
            {
				rotateDirection = new Vector2(velocity.y, velocity.x) * turnMultiplayer;
            }
            else
            {
				rotateDirection = new Vector2(-velocity.y, velocity.x) * turnMultiplayer;
            }

            rotating = true;
        }
    }

    /// <summary> Move the Ship up, down, left or right </summary>
    public void StrafeShip(Vector2 velocity)
    {
        if (velocity.sqrMagnitude >= 1)
        {
            strafeDirection = velocity;
            strafing = true;
        }
        else
        {
            strafing = false;
        }
    }

    /// <summary> Rotate the ship on the Z axis </summary>
    public void RotateShip(float direction)
    {
        rollInput = direction;
    }
	
	/// <summary> Swap the ships weapon  </summary>
	public void SwapWeapon(Vector2 dInput)
	{
		if (dInput.x > 0)
		{
			if (gunController.CanSwap && ammoCount.HasAmmo(WeaponType.Laser))
			{
				currentWeapon = WeaponType.Laser;
				gunController.SwapWeapon(currentWeapon);
			}
		}
		else if (dInput.x < 0)
		{
			if (gunController.CanSwap && ammoCount.HasAmmo(WeaponType.Missiles))
			{
				currentWeapon = WeaponType.Missiles;
				gunController.SwapWeapon(currentWeapon);
			}
		}
		else if (dInput.y > 0)
		{
			if (gunController.CanSwap && ammoCount.HasAmmo(WeaponType.Charged))
			{
				currentWeapon = WeaponType.Charged;
				gunController.SwapWeapon(currentWeapon);
			}
		}
		else if (dInput.y < 0)
		{
			if (gunController.CanSwap)
			{
				// If were using the regular weapon swap to the energy varient, else use the regular one
				currentWeapon = currentWeapon == WeaponType.Regular ? WeaponType.Energy : WeaponType.Regular;
				gunController.SwapWeapon(currentWeapon);
			}
		}

	}

	/// <summary> Make the ship shoot </summary>
	public void Shoot(PlayerRole role, bool pressed)
	{
		if (role == PlayerRole.Pilot)
		{
			shooting_Pilot = pressed;
		}

		if (role == PlayerRole.Gunner)
		{
			if (gunController.CanAttack)
			{
				gunController.UpdateAttacking(pressed);
				shooting_Gunner = pressed;
			}
		}
	}



	/// <summary> Change the roles of the players </summary>
	public void SwapRoles()
	{
		Debug.Log("Swapping roles");

		if (player1)
		{
			player1.SwapRole();
		}

		if (player2)
		{
			player2.SwapRole();
		}

		gunVelocity = Vector3.zero;
	}



	/// <summary> Trigger a role swap request </summary>
	public void TriggerRoleSwap(bool pressed)
	{
		// If we only have one player..
		// Just let them swap on command
		if (numberOfPlayers == 1 && pressed == true)
		{
			SwapRoles();
			return;
		}

		// If were currently looking for a role swap
		if (roleSwap == true && pressed == true)
		{
			// This means that BOTH players are holding down the role swap button
			SwapRoles();
		}
		else
		{
			// Else we want to set looking for roleSwap to whatever the last input was

			// This could be a button press -> start a roleSwap
			// This could be a button release -> cancel the roleSwap

			roleSwap = pressed;
		}
	}

	/// <summary> Toggle the ships map </summary>
	public void ToggleMap(bool pressed)
	{
		// TODO -- ToggleMap();
	}

	/// <summary> Toggle the lockOn member </summary>
	public void LockOn(bool pressed)
	{
	}

	/// <summary> Toggle the boosting member </summary>
	public void Boost(bool pressed)
    {
        // If our boost gauge is more than 5% full we allow the player to boost
        // This is to avoid stuttering  on a 0% gauge
        if (boostGauge > myStats.maxBoostGauge * 0.1f)
		{
			boosting = pressed;
        }
        else
		{
            boosting = false;
        }
    }

	#endregion

	// Updates the UI / HUD
	#region Stat Methods

	public void UpdateBoostGauge()
	{
		// Boost Gauge
		if (boostGauge < myStats.maxBoostGauge && boosting == false)
		{
			float rechargeRate = 1.5f;

			// Recharge our boost gauge
			boostGauge += rechargeRate * Time.deltaTime;

			// Clamp the boost gauge to our maxBoostGauge
			if (boostGauge > myStats.maxBoostGauge)
			{
				boostGauge = myStats.maxBoostGauge;
			}

			// Boost Slider
			slider_Boost.value = (1 / myStats.maxBoostGauge) * boostGauge;

			// Set the color of the boost slider
			boostImage.color = Color.Lerp(Color.red, Color.yellow, (1 / myStats.maxBoostGauge) * boostGauge);
		}
	}
	public void UpdateHealthAndShields()
	{
		slider_Health.value = (1 / shipHP.MaxLife) * shipHP.currentLife;
		slider_Shield.value = (1 / shipHP.MaxShield) * shipHP.currentShield;
	}

	#endregion
}
