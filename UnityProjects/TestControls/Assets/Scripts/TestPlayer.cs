using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is a protoype for flying mechanics.
/// </summary>

[RequireComponent(typeof(Rigidbody))]
public class TestPlayer : MonoBehaviour
{
    Rigidbody rigidbody;
    public float currentSpeed = 0;
    [Space(10)]
    public float minSpeed = 5f;
    public float maxSpeed = 50f;
    public float acceleration = 10f;
    public float deceleration = 10f;
    public float spinSpeed = 50f;
    public float rotateSpeed = 30f;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        float accel = Input.GetAxis("Vertical");
        float rot = Input.GetAxis("SpinAxis");

        //Acceleration and deceleration
        if(accel > 0)
        {
            currentSpeed += accel * acceleration * Time.deltaTime;
        }
        else
        {
            currentSpeed += -deceleration * Time.deltaTime;
        }
        

        // Keeps ship moving and in speed ranges
        if (currentSpeed < minSpeed)
        {
            currentSpeed = minSpeed;
        }
        else if (currentSpeed > maxSpeed)
        {
            currentSpeed = maxSpeed;
        }

        //Move forward
        rigidbody.velocity = transform.forward * currentSpeed;

        //Rotation
        transform.Rotate(new Vector3(0, 0, rot * spinSpeed * Time.deltaTime), Space.Self);
        transform.Rotate(new Vector3(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0) * Time.deltaTime * rotateSpeed);

        Debug.DrawRay(transform.position, transform.forward);

        //rigidbody.velocity += transform.forward *(Time.deltaTime * minSpeed * 100f);

        //// Get the players INPUT
        //float inputX = Input.GetAxis("Horizontal");
        //float inputY = Input.GetAxis("Vertical");
        ////float inputZ = Input.GetAxis("Spin");

        //// GET the rotation speed to moving UP and DOWN
        //float xRot = -inputY * Time.deltaTime * 10f * rotationSpeed.y;

        //// GET the rotation speed to moving LEFT and RIGHT
        //float yRot = inputX * Time.deltaTime * 10f * rotationSpeed.x;

        //// GET the rotation speed to SPIN
        ////float zRot = -inputZ * Time.deltaTime * 10f * rotationSpeed.z;

        //// Set the new ROTATION of the ship
        //transform.localRotation = transform.localRotation * Quaternion.Euler(xRot, yRot, 0);
    }
}
