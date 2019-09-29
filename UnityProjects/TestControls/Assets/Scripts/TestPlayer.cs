using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is a protoype for flying mechanics. It uses a velocity system to keep momentum of flight. You will need to take the physics into account when flying.
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
        currentSpeed = rigidbody.velocity.magnitude;

        float accel = Input.GetAxis("Vertical");
        float rot = Input.GetAxis("SpinAxis");

        //Move forward
        if (currentSpeed <= maxSpeed && currentSpeed >= minSpeed)
        {

            if (accel > 0)
            {
                rigidbody.velocity = rigidbody.velocity + (transform.forward * accel * acceleration * Time.deltaTime);
            }
            else if(accel < 0)
            {
                rigidbody.velocity = rigidbody.velocity + (transform.forward * accel * deceleration * Time.deltaTime);
            }
        }
        else
        {
            rigidbody.velocity = transform.forward * minSpeed;
        }
        

        //Rotation
        transform.Rotate(new Vector3(0, 0, rot * spinSpeed * Time.deltaTime), Space.Self); //Spinning rotation
        transform.Rotate(new Vector3(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0) * Time.deltaTime * rotateSpeed);   //Look rotation

        Debug.DrawRay(transform.position, transform.forward);
    }
}
