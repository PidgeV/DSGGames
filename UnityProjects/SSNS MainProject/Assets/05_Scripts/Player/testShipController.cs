using SNSSTypes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// TODO -- Link Health Shield and Boost Sliders
// TODO -- Menu Navigation
// TODO -- Player Score
// TODO -- Leaving Play Area
// TODO -- Press A to Join
// TODO -- Mouse controls
// TODO -- Less movemement flexibility while boosting
// ect..

// NOTE -- Scriptable Object
// -> ShipStats this ships stats (Max HP, Speed ect)
// -> ShipBehaviour this ships (model) movements (How the model moves)

// NOTE -- Rigidbody Ref [ Mass (500), Drag (20), Angular Drag (10) ]

[RequireComponent(typeof(Rigidbody))]
public class testShipController : MonoBehaviour
{
	/// <summary> The transformation of the ship model </summary>
	[Header("Ship Model")]
	[SerializeField] private Transform ship;

	/// <summary> Reference to the pilot and the gunner </summary>
	[Header("Players")]
	[HideInInspector] public Player player1;
	[HideInInspector] public Player player2;

	/// <summary> [Reference] My Cameras </summary>
	[Header("Cameras")]
	[SerializeField] private Camera pilotCamera;
	[SerializeField] private Camera gunnerCamera;


	[Header("Sliders")]
	[SerializeField] private Slider slider_Health;
	[SerializeField] private Slider slider_Shield;
	[SerializeField] private Slider slider_Boost;

	private Image healthImage;
	private Image shieldImage;
	private Image boostImage;

	/// <summary> [Reference] My Rigidbody </summary>
	Rigidbody rigidbody;

	/// <summary> Reference to the stats for this ship </summary>
	[Header("Ship Stats")]
	public ShipStats myStats;
	public ShipBehaviour myBehaviour;

	// number of people controlling this ship
	int numberOfPlayers = 0;
    public float speed;

    #region Ship Properties

    private HealthAndShields shipHP;

	private float thrustSpeed;
	private float strafeSpeed;
	private float rotationSpeed;
	private float rollDirection;
	private float rollInput;
	private float rollSpeed;
	private float spinSpeed;

	private Vector2 strafeDirection;
	private Vector2 rotateDirection;

	private Vector3 finalStrafeVelocity;
	private Vector3 finalThrustVelocity;
	private Vector3 finalRollRotation;
	private Vector3 finalRotation;

	private bool strafing;
	private bool rotating;
	private bool boosting;
	private bool roleSwap;
	private bool shooting;

	[Header("Controls")]
    [SerializeField] bool invertedControls;
    [SerializeField] bool limitRotation;
    [SerializeField] bool unlimitedBoost;

    // Our boost gauge value
    float boostGauge = 0f;

	// the counter to our next shot
	float shotCounter = 0f;

	//// The ships rotation speed
	float gunRotationSpeed = 75f;

	// Gun Input
	Vector2 gunVelocity;
	Vector2 gunRotation;

	#endregion

	// Initialization
	private void Awake()
	{
		// Set the cameras size and positions
		gunnerCamera.rect = new Rect(0, 0.0f, 1.0f, 0.5f);
		pilotCamera.rect = new Rect(0, 0.5f, 1.0f, 0.5f);

		// Set the max values of our boost Gauge
		boostGauge = myStats.maxBoostGauge;

		// Get components
		rigidbody = gameObject.GetComponent<Rigidbody>();

		// Get the images for the sliders
		boostImage = slider_Boost.gameObject.GetComponentInChildren<Image>();
		healthImage = slider_Health.gameObject.GetComponentInChildren<Image>();
		shieldImage = slider_Shield.gameObject.GetComponentInChildren<Image>();

        TryGetComponent<HealthAndShields>(out shipHP);
		// Set the max values of our health and shields
		//if (gameObject.TryGetComponent<HealthAndShields>(out HealthAndShields durability))
		//{
		//	// Set the max values
		//	durability.maxShield = stats.maxShield;
		//	durability.maxLife = stats.maxHealth;
		//}
	}

	private void Update()
	{
		#region Gunner Logic

		// Gunner Movement
		Vector3 newGunRot = gunVelocity * Time.deltaTime;

		// We never want the gunner to be upsidedown 
		// relative to the players ship
		if (gunRotation.y < -75 && limitRotation) {
			gunRotation.y = -75;
		}

		if (gunRotation.y < -90) {
			newGunRot.x = -newGunRot.x;
		}

		// Applys stick movement to overall rotation while clamping it so it doesn't clip into into the ship
		gunRotation = new Vector2(gunRotation.x + newGunRot.x, Mathf.Clamp(gunRotation.y + newGunRot.y, -180, 0));

		Quaternion p = Quaternion.Euler(-gunRotation.y, 0f, 0f);
		Quaternion y = Quaternion.Euler(0f, gunRotation.x, 0f);

		gunnerCamera.transform.localRotation = Quaternion.Slerp(gunnerCamera.transform.localRotation, y * p, 20 * Time.deltaTime);

		#endregion

		#region Pilot Logic

		float ship_Acceleration = 25f * Time.deltaTime;
		float ship_Deceleration = 50f * Time.deltaTime;

		float rotation_Acceleration = 5f * Time.deltaTime;
		float rotation_Deceleration = 5f * Time.deltaTime;

		float ship_MinSpeed = 50f;
		float ship_MaxSpeed = 100f;
		float ship_MaxBoostSpeed = 300f;
		float ship_MaxStrafeSpeed = 150f;

		float boost_multiplier = 2.4f;

		float rotation_MaxSpeed = 2f;

        speed = rigidbody.velocity.magnitude;

		#region NumberRef
		//float ship_Acceleration = 25f * Time.deltaTime;
		//float ship_Deceleration = 50f * Time.deltaTime;

		//float rotation_Acceleration = 5f * Time.deltaTime;
		//float rotation_Deceleration = 5f * Time.deltaTime;

		//float ship_MinSpeed = 50f;
		//float ship_MaxSpeed = 100f;
		//float ship_MaxBoostSpeed = 150f;
		//float ship_MaxStrafeSpeed = 150f;

		//float boost_multiplier = 1.4f;

		//float rotation_MaxSpeed = 2f;
		#endregion


		// Steer Ship
		#region Steer Ship

		if (rotating)
		{
			rotationSpeed = Mathf.Clamp(rotationSpeed + rotation_Acceleration, 0, rotation_MaxSpeed);
		}
		else
		{
			rotationSpeed = Mathf.Clamp(rotationSpeed - rotation_Deceleration, 0, rotation_MaxSpeed);
		}

		finalRotation = rotateDirection * rotationSpeed;
		#endregion

		// Rotate Ship
		#region  Rotate Ship

		if (rollInput > 0)
		{
			rollDirection = 1;
			rollSpeed = Mathf.Clamp(rollSpeed + rotation_Acceleration, 0, rotation_MaxSpeed);
		}
		else if (rollInput < 0)
		{
			rollDirection = -1;
			rollSpeed = Mathf.Clamp(rollSpeed + rotation_Acceleration, 0, rotation_MaxSpeed);
		}
		else
		{
			rollSpeed = Mathf.Clamp(rollSpeed - rotation_Deceleration, 0, rotation_MaxSpeed);
		}

		finalRollRotation = new Vector3(0, 0, rollDirection) * rollSpeed;
		#endregion

		// Strafe Ship
		#region Strafe Ship

		if (strafing)
		{
			strafeSpeed = Mathf.Clamp(strafeSpeed + ship_Acceleration * 2f, 0, ship_MaxStrafeSpeed);
		}
		else
		{
			strafeSpeed = Mathf.Clamp(strafeSpeed - ship_Deceleration, 0, ship_MaxStrafeSpeed);
		}
		finalStrafeVelocity = strafeDirection * strafeSpeed;
		#endregion

		// Ship Thrust
		#region Ship Thrust

		// If were boosting
		if (boosting)
		{
            // Increase our speed when boosting
            if (rigidbody.velocity.magnitude < ship_MaxBoostSpeed) thrustSpeed = Mathf.Clamp(thrustSpeed + (ship_Acceleration * boost_multiplier), ship_MinSpeed, ship_MaxBoostSpeed);
            

            // Reduce the boost gauge
            boostGauge = Mathf.Clamp(boostGauge - myStats.boostGaugeConsumeAmount * Time.deltaTime, 0, myStats.maxBoostGauge);
			slider_Boost.value = 1 / myStats.maxBoostGauge * boostGauge;

			// Set the color of the boost slider
			boostImage.color = Color.Lerp(Color.red, Color.yellow, 1 / myStats.maxBoostGauge * boostGauge);

			// Turn off boosting
			if (boostGauge <= 0 && unlimitedBoost == false) {
				boosting = false;
			}
		}
		// Else if we are NOT boosting and our thrustSpeed is over our maxThrustSpeed
		// We need to smooth the transition from boosting to not boosting
		else if (thrustSpeed > ship_MaxSpeed)
		{
			// Clamp our thrust Speed to our max thrust speed
			thrustSpeed = Mathf.Clamp(thrustSpeed - ship_Deceleration, ship_MaxSpeed, ship_MaxBoostSpeed);
		}
		// Else were not boosting so we increase our ships normal speed
		else
		{
			thrustSpeed = Mathf.Clamp(thrustSpeed - ship_Deceleration, ship_MinSpeed, ship_MaxSpeed);
		}

		finalThrustVelocity = transform.forward * thrustSpeed;
		#endregion

		#endregion

		// Move the ship model
		UpdateShipModel(rotateDirection);

		// Boost Gauge 
		if (boostGauge < myStats.maxBoostGauge && boosting == false)
		{
			float rechargeRate = 1.5f;

			// Recharge our boost gauge
			boostGauge += rechargeRate * Time.deltaTime;

			// Clamp the boost gauge to our maxBoostGauge 
			if (boostGauge > myStats.maxBoostGauge) {
				boostGauge = myStats.maxBoostGauge;
			}

			// Boost Slider
			slider_Boost.value = (1 / myStats.maxBoostGauge) * boostGauge;

			// Set the color of the boost slider
			boostImage.color = Color.Lerp(Color.red, Color.yellow, (1 / myStats.maxBoostGauge) * boostGauge);
		}

		slider_Health.value = (1 / shipHP.MaxLife) * shipHP.life;
		slider_Shield.value = (1 / shipHP.MaxShield) * shipHP.shield;

		// Shooting
		if (shooting)
		{
			if (shotCounter > myStats.fireRate) {
				Shoot();
			}
		}

		// Increment the shooter timer
		shotCounter += Time.deltaTime;
	}

	private void FixedUpdate()
	{
		// Apply physics to rigidbody
		Quaternion rollRotation = Quaternion.Euler(finalRollRotation);
		Quaternion rotateRotation = Quaternion.Euler(finalRotation);

		rigidbody.rotation *= rollRotation * rotateRotation;

		rigidbody.AddForce(finalThrustVelocity, ForceMode.VelocityChange);
		rigidbody.AddRelativeForce(finalStrafeVelocity, ForceMode.VelocityChange);
	}

	/// <summary> 
	/// Make the ship shoot 
	/// </summary>
	public void Shoot()
	{
		//Debug.Log("Shoot!");

		shotCounter = 0f;
	}

	/// <summary> 
	/// Move the ships model so it looks more like your controlling the ship
	/// </summary>
	public void UpdateShipModel(Vector3 velocity)
	{
		// Rotation	
        Quaternion currentRot = ship.transform.localRotation;
        Quaternion targetRot = Quaternion.Euler
			(
				 velocity.x * myBehaviour.xRot,
				 velocity.z * myBehaviour.yRot,
				-velocity.y * myBehaviour.zRot
			);

		ship.transform.localRotation = Quaternion.Slerp(currentRot, targetRot, myBehaviour.RotSpeed);

		// Translation
        Vector3 currentPos = ship.transform.localPosition;
		Vector3 targetPos = new Vector3(-velocity.y, velocity.x, 0) * myBehaviour.moveScale;

        if (targetPos.y < 0) {
            targetPos.y *= 0.4f;
        }

		ship.transform.localPosition = Vector3.Lerp(currentPos, targetPos, myBehaviour.moveSpeed);

		// Camera
		Vector3 targetCameraPos = boosting ? myBehaviour.boostPos : myBehaviour.normalPos;

		pilotCamera.transform.localPosition = Vector3.Slerp(pilotCamera.transform.localPosition, targetCameraPos, myBehaviour.cameraSpeed);
	}

	/// <summary> 
	/// Move the gunners camera
	/// </summary>
	public void AimGun(Vector2 velocity)
	{
		gunVelocity = new Vector2(velocity.x, velocity.y) * gunRotationSpeed;
	}

	/// <summary> 
	/// Steer the ship 
	/// </summary>
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
				rotateDirection = new Vector2(velocity.y, velocity.x);
			}
			else
			{
				rotateDirection = new Vector2(-velocity.y, velocity.x);
			}

			rotating = true;
		}
	}

	/// <summary>
	/// Move the Ship up, down, left or right
	/// </summary>
	/// <param name="velocity"></param>
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

	/// <summary> 
	/// Rotate the ship on the Z axis
	/// </summary>
	public void RotateShip(float direction)
	{
		rollInput = direction;
	}

	/// <summary>
	/// Trigger a role swap reques
	/// t </summary>
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

	/// <summary>
	/// Toggle the ships map
	/// </summary>
	public void SwapWeapon(Vector2 velocity)
	{
		// TODO -- SpawnWeapon();
	}

	/// <summary>
	/// Toggle the ships map 
	/// </summary>
	public void ToggleMap(bool pressed)
	{
		// TODO -- ToggleMap();
	}

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
			player2 = newPlayer;
			player2.myRole = PlayerRole.Gunner;
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
	/// Change the roles of the players
	/// </summary>
	public void SwapRoles()
	{
		Debug.Log("Swapping roles");

		if (player1) {
			player1.SwapRole();
		}

		if (player2) {
			player2.SwapRole();
		}

		gunVelocity = Vector3.zero;
	}

	/// <summary> 
	/// Make the ship shoot 
	/// </summary>
	public void Shoot(bool pressed)
	{
		shooting = pressed;
	}

	/// <summary> 
	/// Make the ship boost 
	/// </summary>
	public void Boost(bool pressed)
	{
		// If our boost gauge is more than 5% full we allow the player to boost
		// This is to avoid stuttering  on a 0% gauge
		if (boostGauge > myStats.maxBoostGauge * 0.1f) {
			boosting = pressed;
		}
		else
		{
			boosting = false;
		}
	}
}
