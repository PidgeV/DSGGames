using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blackhole : MonoBehaviour
{
    List<GameObject> gos = new List<GameObject>();
    [SerializeField] float rotBase = 0.1f;
    [SerializeField] float rotTime = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(MoveGOs());    
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation,
            Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z + rotBase * Time.deltaTime), rotTime);
        //transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z + 1);
        foreach (GameObject go in gos)
        {
            if (go.TryGetComponent(out Rigidbody rb))
            {
                //rb.AddForce(transform.forward.normalized * 100, ForceMode.Force);
                go.transform.RotateAround(gameObject.transform.position, Vector3.up, rotBase * Time.deltaTime);
            }
        }
    }

    IEnumerator MoveGOs()
    {
        while (true)
        {
            foreach (GameObject go in gos)
            {
                if (go.TryGetComponent<Rigidbody>(out Rigidbody rb))
                {
                    rb.AddForce(transform.forward.normalized*100, ForceMode.Force);
                }
            }
            yield return new WaitForSeconds(0.5f);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        gos.Add(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        gos.Remove(other.gameObject);
    }
}
