using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterEnemy : InterceptCalculationClass
{
    [Header("Green: forward direction and collision check.")]
    [Header("Red: intercept calculation.")]
    [Header("Blue: Velocity and collision check.")]
    public bool debugDraw = false;

    [Space(15)]
    public GameObject player;
    public GameObject bulletPrefab;
    [Tooltip("Enemy will fly to these in preperation for its next attempt")]
    public GameObject[] resetPositions;
    public LayerMask obstacleLayer;
    public float collisionCheckDistance = 100f;
    [Space(15)]
    [Tooltip("The speed the ship will travel in meters per second")]
    public float speed = 150f;
    [Space(15)]
    [Tooltip("The acceleration of the ship in meters per second")]
    public float acceleration = 20f;
    [Tooltip("The speed the ship will rotate")]
    public float rotationForce = 1f;

    [Space(15)]
    [Tooltip("Distance to target to allow ship to choose a new target.")]
    public float distanceToTarget = 50;
    [Tooltip("Time in seconds between trajectory calculations")]
    public float calculateInterval = 0.1f;

    #region Shooting
    [Space(15)]
    [Tooltip("The distance in units that the enemy is willing to shoot. Will not shoot outside this distance")]
    public float accuracy = 5f;
    public GameObject bulletSpawnPos;
    [Tooltip("How long it takes for the enemy to shoot")]
    public float shotInterval = 1f;
    float shotTimer = 0;
    #endregion

    GameObject target; // GameObject the enemy will fly towards. public for testing purposes only
    Rigidbody rbTarget;
    Rigidbody rbSelf;
    Vector3 interceptPoint;

    #region Obstacle_variables
    Vector3 obstacleAvoidDirection = Vector3.right;
    bool obstacleHit = false;
    float obstacleTimer = 0;
    public float avoidTime = 2f;
    [Tooltip("Size of ray for collision checking. Larger numbers will mean the avoidance is larger")]
    public float raySize = 7.5f;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        target = resetPositions[Random.Range(0, resetPositions.Length)];
        rbSelf = GetComponent<Rigidbody>();
        rbTarget = target.GetComponent<Rigidbody>();

        StartCoroutine(CalculateIntercept());
    }

    // Update is called once per frame
    void Update()
    {
        ChooseTarget();
        Move();
        Shoot();
    }

    //Calculates the intercept point
    IEnumerator CalculateIntercept()
    {
        while (true)
        {
            yield return new WaitForSeconds(calculateInterval);
            //positions
            Vector3 targetPosition = target.transform.position;
            //velocities
            Vector3 velocity = rbSelf ? rbSelf.velocity : Vector3.zero;
            Vector3 targetVelocity = rbTarget ? rbTarget.velocity : Vector3.zero;

            //calculate intercept
            interceptPoint = FirstOrderIntercept(transform.position, velocity, bulletPrefab.GetComponent<Bullet>().speed, targetPosition, targetVelocity);
        }
    }

    void Move()
    {
        if (target != null)
        {
            //Calculate direction
            Vector3 direction = transform.forward; // sets forward
            direction.Normalize();

            AvoidObstacles(ref direction); // will change direction towards the right if an obstacle is in the way

            //rotation
            if (!obstacleHit && obstacleTimer == 0)
            {
                direction = interceptPoint - transform.position; // sets desired direction to target intercept point

                //if no obstacles, allow rotation to desired direction
                Quaternion rotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationForce * Time.deltaTime);
            }
            else
            {
                //if obstacles, ignore desired direction and move to the right of obstacles
                obstacleTimer += Time.deltaTime;
                if (obstacleTimer > avoidTime)
                {
                    obstacleTimer = 0;
                    obstacleHit = false;
                }

                Quaternion rot = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, rot, rotationForce * Time.deltaTime);
            }

            //Move
            rbSelf.AddForce(transform.forward.normalized * acceleration);

            if (rbSelf.velocity.magnitude >= speed)
            {
                rbSelf.velocity = rbSelf.velocity.normalized * speed; //Limit velocity to speed
            }
        }
    }

    void AvoidObstacles(ref Vector3 dir)
    {
        RaycastHit hitInfo;

        //Check direction facing
        if (Physics.SphereCast(transform.position, raySize, transform.forward.normalized, out hitInfo, collisionCheckDistance, obstacleLayer))
        {
            // Get the desired direction we need to move to move around  the obstacle. Transform to world co-ordinates (gets the obstacleMoveDirection wrt the current foward direction).
            Vector3 turnDir = transform.TransformDirection(obstacleAvoidDirection);
            turnDir.Normalize();

            dir += turnDir;
            obstacleHit = true;
        }

        //Check where velocity is moving ship
        else if (Physics.SphereCast(transform.position, raySize, rbSelf.velocity.normalized, out hitInfo, collisionCheckDistance, obstacleLayer))
        {
            // Get the desired direction we need to move to move around  the obstacle. Transform to world co-ordinates (gets the obstacleMoveDirection wrt the current foward direction).
            Vector3 turnDir = transform.TransformDirection(obstacleAvoidDirection);
            turnDir.Normalize();

            dir += turnDir;
            obstacleHit = true;
        }
    }

    void ChooseTarget()
    {
        if (target != null)
        {
            if (Vector3.Distance(transform.position, target.transform.position) <= distanceToTarget && resetPositions.Length > 0) // change target when close enough
            {
                if (target.Equals(player)) target = resetPositions[Random.Range(0, resetPositions.Length)]; //choose random one from list if player is current target
                else if (player != null) target = player;   //Otherwise choose player
            }
        }
        else
        {
            target = resetPositions[Random.Range(0, resetPositions.Length)];
        }
    }

    void Shoot()
    {
        if (target.Equals(player))
        {
            shotTimer += Time.deltaTime;

            Quaternion lookRot = Quaternion.LookRotation(transform.forward);
            float distance = Vector3.Distance(interceptPoint, transform.position);
            Vector3 predictedShotPos = transform.position + (transform.forward.normalized * distance);
            float distanceFromPath = Vector3.Cross(transform.forward, interceptPoint - transform.position).magnitude;

            if (distanceFromPath <= accuracy && shotTimer >= shotInterval)
            {
                GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPos.transform.position, lookRot);

                shotTimer = 0;
            }
        }
    }

    //Draw debug rays
    private void OnDrawGizmos()
    {
        if (debugDraw)
        {
            Debug.DrawRay(transform.position, transform.forward.normalized * collisionCheckDistance, Color.green); //Forward ray
            Debug.DrawRay(transform.position, interceptPoint - transform.position, Color.red); // Intercept point
            if (rbSelf)
            {
                //Vector3 dir = rbSelf.velocity;
                Debug.DrawRay(transform.position, rbSelf.velocity.normalized * collisionCheckDistance, Color.blue); //Velocity ray
            }
        }
    }
}