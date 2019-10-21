using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Quick hard coded sample chaser enemy. Inherits intercept class to determine where to move to. Utilizes a rigidbody to propel so that colliders will function if it hits stuff

[RequireComponent(typeof(Rigidbody))]
public class ChaserEnemy : InterceptCalculationClass
{
    [Header("Green: forward direction and collision check.")]
    [Header("Red: intercept calculation.")]
    [Header("Blue: Velocity and collision check.")]
    public bool debugDraw = false;
    
    [Space(15)]
    public GameObject player;
    [Tooltip("Enemy will fly to these in preperation for its next attempt")]
    public GameObject[] resetPositions;
    public LayerMask obstacleLayer;
    public float collisionCheckDistance = 150f;
    [Space(15)]
    [Tooltip("The speed the ship will travel in meters per second")]
    public float regularSpeed = 150f;
    public float chargeSpeed = 400f;
    [Space(15)]
    [Tooltip("The acceleration of the ship in meters per second")]
    public float acceleration = 20f;
    public float chargeAcceleration = 50f;
    [Tooltip("The speed the ship will rotate")]
    public float rotationForce = 1f;
    
    [Space(15)]
    [Tooltip("Distance to target to allow ship to choose a new target.")]
    public float distanceToTarget = 15;
    [Tooltip("Time in seconds between trajectory calculations")]
    public float calculateInterval = 0.5f;
    [Tooltip("Will make damage the current speed divided by this number.")]
    public float damageDivision = 4;

    GameObject target; // GameObject the enemy will fly towards. public for testing purposes only
    Rigidbody rbTarget;
    Rigidbody rbSelf;
    Vector3 interceptPoint;

    //Obstacle variables
    Vector3 obstacleAvoidDirection = Vector3.right;
    bool obstacleHit = false;
    float obstacleTimer = 0;
    public float avoidTime = 2f;
    [Tooltip("Size of ray for collision checking. Larger numbers will mean the avoidance is larger")]
    public float raySize = 7.5f;

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

        GetComponent<Damage>().ChangeDamage(rbSelf.velocity.magnitude / damageDivision); //  changes damage to the speed it is travelling at during collision
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
            interceptPoint = FirstOrderIntercept(transform.position, velocity, rbSelf.velocity.magnitude, targetPosition, targetVelocity);
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
            if (target.Equals(player) && !obstacleHit)
            {
                rbSelf.AddForce(transform.forward.normalized * chargeAcceleration);

                if (rbSelf.velocity.magnitude >= chargeSpeed)
                {
                    rbSelf.velocity = rbSelf.velocity.normalized * chargeSpeed;
                }
            }
            else
            {
                rbSelf.AddForce(transform.forward.normalized * acceleration);

                if (rbSelf.velocity.magnitude >= regularSpeed)
                {
                    rbSelf.velocity = rbSelf.velocity.normalized * regularSpeed;
                }
            }
        }
    }

    void AvoidObstacles(ref Vector3 dir)
    {
        RaycastHit hitInfo;

        //Check direction facing
        if (Physics.SphereCast(transform.position, 5,transform.forward, out hitInfo, collisionCheckDistance, obstacleLayer))
        {
            // Get the desired direction we need to move to move around  the obstacle. Transform to world co-ordinates (gets the obstacleMoveDirection wrt the current foward direction).
            Vector3 turnDir = transform.TransformDirection(obstacleAvoidDirection);
            turnDir.Normalize();

            dir += turnDir;
            obstacleHit = true;
        }

        //Check where velocity is moving ship
        else if (Physics.SphereCast(transform.position, 5, rbSelf.velocity, out hitInfo, collisionCheckDistance, obstacleLayer))
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

    //Draw debug rays
    private void OnDrawGizmos()
    {
        if (debugDraw)
        {
            Debug.DrawRay(transform.position, transform.forward.normalized * collisionCheckDistance, Color.green); //Forward ray
            Debug.DrawRay(transform.position, interceptPoint - transform.position, Color.red); // Intercept point
            if(rbSelf) Debug.DrawRay(transform.position, rbSelf.velocity, Color.blue); //Velocity ray
        }
    }
}
