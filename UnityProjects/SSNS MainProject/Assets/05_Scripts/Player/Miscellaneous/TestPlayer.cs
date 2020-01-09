using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    [Range(0.2f, 2.0f)]
    public float rotateSpeed = 1.0f;

    [Space(10)]
    public RingControl ringControl;

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

        //Move forward
        if (currentSpeed <= maxSpeed && currentSpeed >= minSpeed)
        {
            if (accel > 0)
            {
                currentSpeed += accel * acceleration * Time.deltaTime;
            }
            else
            {
                currentSpeed += accel * deceleration * Time.deltaTime;
            }
        }
        else
        {
            if (currentSpeed <= minSpeed)
            {
                currentSpeed = minSpeed;
            }
            else
            {
                currentSpeed = maxSpeed;
            }
        }

        //Rotation
        transform.Rotate(new Vector3(0, 0, rot * spinSpeed * Time.deltaTime), Space.Self); //Spinning rotation

        if (ringControl) // Ring rotation
        {
            Vector2 ringDir = ringControl.GetShipRotation;
            transform.Rotate(ringDir * rotateSpeed * Time.deltaTime);
        }

        rigidbody.velocity = transform.forward * currentSpeed;

        Debug.DrawRay(transform.position, transform.forward * 100, Color.red);
    }

    private void OnGUI()
    {
        //GUI.color = Color.white;
        //GUI.Label(new Rect(5, 5, 200, 50), "Current Speed = " + currentSpeed);
    }
}
