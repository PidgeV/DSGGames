using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrdPieceRepulsion : MonoBehaviour
{
    [SerializeField] Transform dreadnova;
    [SerializeField] float propulsionForce = 100f;
    [SerializeField] bool yFire = true;
    float spinnyAmount = 1.0f;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<HealthAndShields>().onDeath += Repulse;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Repulse()
    {
        DrdRandomShooty.manager.AddShoot();

        gameObject.GetComponent<DrdFireArea>().SetGlowy(false);

        gameObject.TryGetComponent(out Rigidbody rb);

        if (rb != null)
        {
            Vector3 dir = gameObject.transform.position - dreadnova.position;
            dir = dir.normalized;
            if(yFire)
                dir = new Vector3(dir.x, dir.y * 2, 0);
            else
                dir = new Vector3(dir.x * 2, dir.y, 0);

            rb.AddForce(dir * propulsionForce, ForceMode.Impulse);
            rb.AddTorque( spinnyAmount * Random.Range(0.0f, 1.0f), spinnyAmount * Random.Range(0.0f, 1.0f), spinnyAmount * Random.Range(0.0f, 1.0f), ForceMode.Impulse);
        }
    }
}
