using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is a protoype for flying mechanics. It uses a velocity system to keep momentum of flight. You will need to take the physics into account when flying.
/// </summary>

[RequireComponent(typeof(Rigidbody))]
public class FlightController : MonoBehaviour
{
    Rigidbody rb;
    public Vector3 currentSpeed;
    [Space(10)]
    public float minSpeed = 5f;
    public float maxSpeed = 50f;
    public float acceleration = 10f;
    public float deceleration = 10f;
    public float strafeSpeed = 5f;
    public float spinSpeed = 50f;
    public float rotateSpeed = 30f;
    public float brakeDrag = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        currentSpeed = rb.velocity;

        float accel = Input.GetAxis("Vertical"); //Gets forward and reverse axis
        float rot = Input.GetAxis("SpinAxis"); // Gets spin axis
        float strafe = Input.GetAxis("Horizontal");
        float brake = Input.GetAxis("Brake");

        //Movement if speed is within bounds
        if (currentSpeed.magnitude <= maxSpeed && currentSpeed.magnitude >= minSpeed)
        {
            //Forwad movement
            if (accel > 0)
            {
                rb.AddForce(transform.forward * accel * acceleration);
            }
            //Reverse movement
            else if (accel < 0)
            {
                rb.AddForce(transform.forward * accel * deceleration);
            }

            //Strafing
            rb.AddForce(transform.right * strafe * strafeSpeed);

            Vector3 newPos = transform.position + transform.right * strafe;
            Vector3.MoveTowards(transform.position, newPos, strafeSpeed);

        }
        //Move the minimum speed if outside range
        else 
        {
            rb.velocity = transform.forward * minSpeed;
        }

        //Braking
        if (brake != 0)
        {
            rb.drag = brakeDrag;
        }
        else
        {
            rb.drag = 0;
        }

        //Rotation
        transform.Rotate(new Vector3(0, 0, rot * spinSpeed * Time.deltaTime), Space.Self); //Spinning rotation
        transform.Rotate(new Vector3(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0) * Time.deltaTime * rotateSpeed);   //Look rotation

        
        //transform.Translate(transform.right * strafe * strafeSpeed * Time.deltaTime);   WONT MOVE IN PROPER DIRECTION. GOES IN RANDOM DIRECTIONS

        Debug.DrawRay(transform.position, transform.forward);
    }
}