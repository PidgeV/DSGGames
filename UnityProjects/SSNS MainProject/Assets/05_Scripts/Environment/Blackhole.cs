using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blackhole : MonoBehaviour
{
    List<GameObject> gos = new List<GameObject>();
    [SerializeField] float rotBase = 10.0f;
    [SerializeField] float rotTime = 3.0f;
    [SerializeField] float pullForce = 0.001f;
    [SerializeField] float DOT = 5.0f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation,
            Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z + rotBase * Time.deltaTime), rotTime);

    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Rigidbody rb))
        {
            other.gameObject.transform.RotateAround(gameObject.transform.position, Vector3.up, rotBase * Time.deltaTime);

            other.gameObject.transform.position = Vector3.Lerp(other.gameObject.transform.position, transform.position, pullForce);

        }
        if (other.gameObject.TryGetComponent(out HealthAndShields has))
        {
            has.TakeDamage(DOT * 0.6f * Time.fixedDeltaTime, DOT * 0.4f * Time.fixedDeltaTime);
        }
    }
}
