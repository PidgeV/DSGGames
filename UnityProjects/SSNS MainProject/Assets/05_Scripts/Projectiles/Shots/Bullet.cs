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

    // The speed the projectile moves
    [Tooltip("Speed in m/s")]
    public float speed = 50.0f;

    private void Awake()
    {
        //Propel bullet on spawn
        Vector3 rotation = transform.forward;
        rotation.Normalize();

        Rigidbody rb = GetComponent<Rigidbody>();
        rb.velocity = rotation * speed; // propel forward with set speed
    }
}
