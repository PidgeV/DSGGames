using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MountRotate : InterceptCalculationClass
{
    public bool debug = false;

    public GameObject bulletPrefab;
    public float threatDistance = 200f;
    public float rotateSpeed = 0.1f;

    public float calculateInterval = 0.2f;

    GameObject target;
    GameObject player;

    Rigidbody rbTarget;
    Vector3 interceptPoint;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        StartCoroutine(CalculateIntercept());
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, player.transform.position) < threatDistance)
        {
            Rotate();
        }
        else
        {
            target = null;
        }
    }

    void Rotate()
    {
        target = player;
        rbTarget = target.GetComponent<Rigidbody>();

        Vector3 dir = interceptPoint - transform.position;
        Vector3 rot = Vector3.RotateTowards(transform.forward, dir, rotateSpeed * Time.deltaTime, 0.0f);
        rot.Normalize();

        transform.rotation = Quaternion.LookRotation(rot);

        transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
    }

    IEnumerator CalculateIntercept()
    {
        while (true)
        {
            yield return new WaitForSeconds(calculateInterval);
            //positions
            if (target)
            {
                Vector3 targetPosition = target.transform.position;
                //velocities
                //Vector3 velocity = rbSelf ? rbSelf.velocity : Vector3.zero;
                Vector3 velocity = Vector3.zero;
                Vector3 targetVelocity = rbTarget ? rbTarget.velocity : Vector3.zero;

                //calculate intercept
                interceptPoint = FirstOrderIntercept(transform.position, velocity, bulletPrefab.GetComponent<ShotInfo>().Speed, targetPosition, targetVelocity);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if(debug)
        {
            Debug.DrawRay(transform.position, transform.forward * threatDistance);
        }
    }
}
