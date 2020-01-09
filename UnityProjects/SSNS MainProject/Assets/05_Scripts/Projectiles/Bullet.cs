using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// NOTE -- Any gameObjects with a Bullet script should also have a Damage script
//		   The Damage script holds the damage members for any collidables including bullets

// The Bullet Class keeps track of how long a projectile should stay alive and how / how fast it should move
[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    public GameObject shooter;
    // The time the projectile is alive after its Instantiated
    public float lifetime = 10f;

    // The speed the projectile moves
    [Tooltip("Speed in m/s")]
    public float speed = 50.0f;

    // A counter till the projectile will be destroyed
    float counter = 0.0f;

    private void Start()
    {
        //Propel bullet on spawn
        Vector3 rotation = transform.forward;
        rotation.Normalize();

        Rigidbody rb = GetComponent<Rigidbody>();
        rb.velocity = rotation * speed; // propel forward with set speed
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the projectile has to be destroyed
        if (counter > lifetime)
        {
            Destroy(gameObject);
        }
        else
        {
            counter += Time.deltaTime;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
    }
}
