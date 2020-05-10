using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DreadnovaShotController : MonoBehaviour
{
    [SerializeField] GameObject ChargeEffect;
    [SerializeField] GameObject ChargeShot;
    [SerializeField] float startTime;
    [SerializeField] float chargeDelay = 1f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(StartShot());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator StartShot()
    {
        if (startTime > 0) yield return new WaitForSeconds(startTime);

        ChargeEffect.SetActive(true);

        yield return new WaitForSeconds(chargeDelay);

        ChargeShot.SetActive(true);
    }
}
