using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Quick hard coded sample chaser enemy. Inherits intercept class to determine where to move to. Utilizes a rigidbody to propel so that colliders will function if it hits stuff

[RequireComponent(typeof(Rigidbody))]
public class ChaserEnemy : InterceptCalculationClass
{
    public GameObject player;
    [Tooltip("Enemy will fly to these in preperation for its next attempt")]
    public GameObject[] resetPositions;

    public bool debugDraw = false;
    public LayerMask obstacleLayer;
    public float collisionDistance = 150f;
    [Tooltip("The speed the ship will travel")]
    public float regularSpeed = 150f;
    public float chargeSpeed = 400f;
    public float acceleration = 20f;
    public float chargeAcceleration = 50f;
    [Tooltip("The speed the ship will rotate")]
    public float rotationForce = 1f;

    [Tooltip("Time in seconds between trajectory calculations")]
    public float calculateInterval = 0.5f;

    public GameObject target; // GameObject the enemy will fly towards
    Rigidbody rbTarget;
    Rigidbody rbSelf;
    Vector3 interceptPoint;

    // Start is called before the first frame update
    void Start()
    {
        target = player;
        rbSelf = GetComponent<Rigidbody>();
        rbTarget = target.GetComponent<Rigidbody>();

        StartCoroutine(CalculateIntercept());
    }

    // Update is called once per frame
    void Update()
    {
        ChooseTarget();
        Move();

        GetComponent<Damage>().ChangeDamage(rbSelf.velocity.magnitude); //  changes damage to the speed it is travelling at during collision
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
            Vector3 direction = interceptPoint - transform.position;
            direction.Normalize();

            AvoidObstacles(ref direction);

            //Rotate
            Quaternion rotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationForce * Time.deltaTime);

            //Move
            if (target.Equals(player))
            {
                rbSelf.AddForce(transform.forward.normalized * acceleration);

                if (rbSelf.velocity.magnitude >= chargeSpeed)
                {
                    rbSelf.velocity = rbSelf.velocity.normalized * chargeSpeed;
                }
            }
            else
            {
                rbSelf.AddForce(transform.forward.normalized * chargeAcceleration);

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

        if (Physics.Raycast(transform.position, transform.forward, out hitInfo, collisionDistance, obstacleLayer))
        {
            dir = transform.forward + (hitInfo.normal * rotationForce * Time.deltaTime);
        }
    }

    void ChooseTarget()
    {
        if (target != null)
        {
            if (Vector3.Distance(transform.position, target.transform.position) <= 15 && resetPositions.Length > 0) // change target when close enough
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
            Debug.DrawRay(transform.position, transform.forward * collisionDistance, Color.green); //Forward ray
            Debug.DrawRay(transform.position, interceptPoint - transform.position, Color.red); // Intercept point
        }
    }
}
