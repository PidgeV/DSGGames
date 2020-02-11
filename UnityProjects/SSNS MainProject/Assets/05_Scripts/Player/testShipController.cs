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
    #region Shooting things
    [SerializeField] WeaponType startingWeapon;
    [HideInInspector] public WeaponType currentWeapon;
    private ShotInfo currentShotInfo;
    [SerializeField] ShotInfo[] shots;
    [SerializeField] Transform shotSpawnLocation;

    [SerializeField] LaserBehaviour laser;

    private float shotTimer;
    #endregion

    #region Other Things
    /// <summary> The transformation of the ship model </summary>
    [Header("Ship Model")]
    [SerializeField] private Transform ship;
    [SerializeField] private Transform gunHelper;

    /// <summary> Reference to the pilot and the gunner </summary>
    [Header("Players")]
    [HideInInspector] public Player player1;
    [HideInInspector] public Player player2;

    /// <summary> [Reference] My Cameras </summary>
    [Header("Cameras")]
    [SerializeField] private Camera pilotCamera;
    [SerializeField] private GameObject gunnerCamParent;
    [SerializeField] private Camera gunnerCamera;
    Quaternion gunnerAim = Quaternion.identity;

    private Slider slider_Health;
    private Slider slider_Shield;
    private Slider slider_Boost;

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

    #endregion

    #region Ship Properties
    HealthAndShields shipHP;

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
    private bool lockOn;
    private bool shooting_Gunner;
    private bool shooting_Pilot;

    [Header("Controls")]
    [SerializeField] bool invertedControls;
    [SerializeField] bool limitRotation;
    [SerializeField] bool unlimitedBoost;

    // Our boost gauge value
    float boostGauge = 0f;

    // the counter to our next shot
    float shotCounter = 0f;

    //// The ships rotation speed
    float gunRotationSpeed = 200f;

    // Gun Input
    Vector2 gunVelocity;
    Vector2 gunRotation;

    public AmmoCounter ammoCount;

    #endregion

    private bool stopThrust;

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

        slider_Health = GameObject.Find("[Slider] Health").GetComponent<Slider>();
        slider_Shield = GameObject.Find("[Slider] Shield").GetComponent<Slider>();
        slider_Boost = GameObject.Find("[Slider] Boost").GetComponent<Slider>();

        // Get the images for the sliders
        boostImage = slider_Boost.gameObject.GetComponentInChildren<Image>();
        healthImage = slider_Health.gameObject.GetComponentInChildren<Image>();
        shieldImage = slider_Shield.gameObject.GetComponentInChildren<Image>();

        // Get references
        if (!shipHP) TryGetComponent(out shipHP);

        //
        currentWeapon = startingWeapon;

        //
        foreach (ShotInfo s in shots)
        {
            if (s.weapon == currentWeapon)
            {
                currentShotInfo = s;
            }
        }
    }

    private void OnDestroy()
    {
        if (player1)
            Destroy(player1.gameObject);

        if (player2)
            Destroy(player2.gameObject);
    }

    // Unity Methods and Events
    #region Unity ( Update, FixedUpdate and LateUpdate )

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

        #region Pilot Logic

        speed = rigidbody.velocity.magnitude;


        // Steer Ship
        #region Steer Ship

        if (rotating)
        {
            rotationSpeed = Mathf.Clamp(rotationSpeed + myStats.shipRotAcceleration, 0, myStats.rotationSpeed);
        }
        else
        {
            rotationSpeed = Mathf.Clamp(rotationSpeed - myStats.shipRotAcceleration, 0, myStats.rotationSpeed);
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
            if (rigidbody.velocity.magnitude < myStats.boostSpeed) thrustSpeed = Mathf.Clamp(thrustSpeed + (myStats.thrustSpeed * 2.4f), myStats.thrustSpeed, myStats.boostSpeed);


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
            if (boostGauge > myStats.maxBoostGauge)
            {
                boostGauge = myStats.maxBoostGauge;
            }

            // Boost Slider
            slider_Boost.value = (1 / myStats.maxBoostGauge) * boostGauge;

            // Set the color of the boost slider
            boostImage.color = Color.Lerp(Color.red, Color.yellow, (1 / myStats.maxBoostGauge) * boostGauge);
        }

        slider_Health.value = (1 / shipHP.MaxLife) * shipHP.currentLife;
        slider_Shield.value = (1 / shipHP.MaxShield) * shipHP.currentShield;

        // Shooting
        if (shooting_Gunner)
        {
            Shoot();
        }
        else
        {
            laser.gameObject.SetActive(false);
        }

        if (shooting_Pilot)
        {
            ShipShoot();
        }

        // Increment the shooter timer
        shotCounter += Time.deltaTime;
    }

    /// <summary>
    /// FixedUpdate is used for physic calculations
    /// </summary>
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
    /// LateUpdate is used to set the gunners camera, the reason for this is
    /// because we need to know where the ship is after physics are applied
    /// </summary>
    public GameObject lockOnTarget;
    private void LateUpdate()
    {
        if (lockOn && lockOnTarget)
        {
            float speed;
            if (currentWeapon == WeaponType.Laser)
            {
                speed = Mathf.Infinity;
            }
            else
            {
                speed = currentShotInfo.Speed;
            }

            Vector3 intercept = InterceptCalculationClass.FirstOrderIntercept(shotSpawnLocation.position, Vector3.zero, speed, lockOnTarget.transform.position, lockOnTarget.GetComponent<Rigidbody>().velocity);
            gunnerCamParent.transform.LookAt(intercept);
            //gunRotation = gunnerCamera.transform.eulerAngles;
        }
        else
        {
            // POSITION
            // Where the camera SHOULD be relative to the ship model
            gunnerCamera.transform.position = ship.transform.position + (-transform.up * 7);

            // ROTATION
            // Add the current input to the guns final rotation
            gunnerAim *= Quaternion.Euler(new Vector2(-gunVelocity.y, gunVelocity.x) * Time.deltaTime);
            gunnerCamParent.transform.rotation = gunnerAim;

            //gunRotation += (new Vector2(-gunVelocity.y, gunVelocity.x) * Time.deltaTime);

            //if (gunRotation.x < -80f)
            //{
            //    gunRotation.x = -80f;
            //}

            //if (gunRotation.x > 80f)
            //{
            //    gunRotation.x = 80f;
            //}

            //// Rotate the gun to its correct rotation
            //gunHelper.rotation = Quaternion.identity;
            //gunHelper.Rotate(gunRotation);

            //// TODO -- We need to prevent the gunner from looking up

            //// Set the camera to its target rotation
            //gunnerCamera.transform.LookAt(gunHelper.position + gunHelper.forward * 300);

            RaycastHit hit;
            // Does the ray intersect any objects excluding the player layer
            if (lockOn == false && Physics.Raycast(gunnerCamera.transform.position, gunnerCamera.transform.forward, out hit, Mathf.Infinity))
            {
                if (hit.collider.gameObject.tag == "Enemy" ||
                    hit.collider.gameObject.tag == "Shield")
                {
                    lockOnTarget = hit.collider.gameObject;
                }
            }
        }
    }

    #endregion

    /// <summary>
    /// Make the ship shoot
    /// </summary>
    public void Shoot()
    {
        Ray ray = new Ray(gunnerCamera.transform.position, gunnerCamera.transform.forward);
        LayerMask layer = LayerMask.GetMask("Player");

        //If not hitting player collider
        if (!Physics.Raycast(ray, 15f, layer))
        {
            if (currentWeapon == WeaponType.Laser)
            {
                //LaserShot: Long, straight beam, near instant, aoe from origin
                //Raycast instead of instantiate for instant movement
                ammoCount.Take1Ammo(currentWeapon);

                //Shoot laser here
                laser.gameObject.SetActive(true);

                LayerMask enemyLayer = LayerMask.GetMask("Enemies");
                enemyLayer += LayerMask.GetMask("Swarm");
                RaycastHit[] hits = Physics.SphereCastAll(shotSpawnLocation.transform.position, 15f, shotSpawnLocation.transform.forward.normalized, laser.Length, enemyLayer);

                foreach (RaycastHit hit in hits)
                {
                    if (hit.collider.TryGetComponent(out HealthAndShields hp))
                    {
                        hp.TakeDamage(laser.Damage, laser.Damage);
                    }
                }

                laser.SetLaser(shotSpawnLocation.transform.position + shotSpawnLocation.transform.forward.normalized * laser.Length);
            }
            else if (shotTimer > currentShotInfo.FireRate)
            {
                shotTimer = 0;

                if (ammoCount.HasAmmo(currentWeapon))
                {
                    ammoCount.Take1Ammo(currentWeapon);
                    SpawnShot();
                }
            }
        }
    }

    private void SpawnShot()
    {
        Quaternion rot = Quaternion.LookRotation(shotSpawnLocation.transform.forward);
        GameObject shot = Instantiate(currentShotInfo.gameObject, shotSpawnLocation.position, rot);

        if(TryGetComponent(out ShieldProjector shield))
        {
            shield.IgnoreCollider(shot.GetComponent<Collider>());
        }
    }

    public void ShipShoot()
    {
        if (shotTimer > currentShotInfo.FireRate)
        {
            shotTimer = 0;
            Quaternion rot = Quaternion.LookRotation(transform.forward);
            GameObject shot = Instantiate(currentShotInfo.gameObject, transform.position + transform.forward * 10, Quaternion.identity);
            shot.transform.rotation = rot;
        }
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

        if (targetPos.y < 0)
        {
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
	/// Trigger a role swap request
	/// </summary>
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
    /// Swap the ships weapon
    /// </summary>
    public void SwapWeapon(Vector2 dInput)
    {
        if (dInput.x > 0)
        {
            currentWeapon = WeaponType.Laser;
        }
        else if (dInput.x < 0)
        {
            currentWeapon = WeaponType.Missiles;
        }
        else if (dInput.y > 0)
        {
            //currentWeapon = WeaponType.Charged;
        }
        else if (dInput.y < 0)
        {
            if(currentWeapon == WeaponType.Regular)
            {
                currentWeapon = WeaponType.Energy;
            }
            else
            {
                currentWeapon = WeaponType.Regular;
            }
        }
    }

    /// <summary>
    /// Toggle the ships map
    /// </summary>
    public void ToggleMap(bool pressed)
    {
        // TODO -- ToggleMap();
    }

    public void LockOn(bool pressed)
    {
        lockOn = pressed;
    }

    /// <summary>
	/// Change the roles of the players
	/// </summary>
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

    /// <summary>
	/// Make the ship shoot
	/// </summary>
    public void Shoot(PlayerRole role, bool pressed)
    {
        if (role == PlayerRole.Pilot)
        {
            shooting_Pilot = pressed;
        }

        if (role == PlayerRole.Gunner)
        {
            shooting_Gunner = pressed;
        }
    }

    /// <summary>
	/// Make the ship boost
	/// </summary>
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

    public bool StopThrust { set { stopThrust = value; } }
}
