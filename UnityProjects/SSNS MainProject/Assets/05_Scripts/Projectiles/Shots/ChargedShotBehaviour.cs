﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(ShotInfo))]
public class ChargedShotBehaviour : MonoBehaviour
{
    [Range(1, 15)]
    [SerializeField] float maxChargeTime = 5f;
    
    //Damage values
    [Space(15)]
    [SerializeField] float minDamage = 10f;
    [SerializeField] float maxDamage = 50f;
    private float currentDamage;
    private float increasePerSecond;

    //Scale values
    [Space(15)]
    [SerializeField] float minScale = 0.5f;
    [SerializeField] float maxScale = 2f;
    private Vector3 scalePerSecond;

    private float speed;
    private bool hasShot = false;            

    public bool HasShot
    {
        get { return hasShot; }
        set
        {
            hasShot = value;
            if(value) transform.parent = null;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        currentDamage = minDamage;
        increasePerSecond = (maxDamage - minDamage) / maxChargeTime;
        speed = GetComponent<ShotInfo>().Speed;

        Vector3 scale = new Vector3(minScale, minScale, minScale);
        transform.localScale = scale;

        float scaleSecond = (maxScale - minScale) / maxChargeTime;
        scalePerSecond = new Vector3(scaleSecond, scaleSecond, scaleSecond);

        StartCoroutine(StartCharge());
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasShot)
        {
            currentDamage = Mathf.Clamp(currentDamage + increasePerSecond * Time.deltaTime, minDamage, maxDamage);
            transform.localScale += scalePerSecond * Time.deltaTime;
        }
        else
        {
            transform.position += transform.forward.normalized * speed * Time.deltaTime;
        }
    }

    IEnumerator StartCharge()
    {
        yield return new WaitForSeconds(maxChargeTime);

        transform.parent = null;
        hasShot = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.TryGetComponent(out HealthAndShields health))
        {
            health.TakeDamage(currentDamage, currentDamage);
        }
    }
}
