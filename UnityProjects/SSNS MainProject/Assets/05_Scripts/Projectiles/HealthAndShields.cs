using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// NOTE -- To work you should have a collider on this gameobject WITH IsTrigger set to true
//		   You also need a Rigidbody that is NOT kinematic

[RequireComponent(typeof(Collider))]
public class HealthAndShields : MonoBehaviour
{
    [Space(10)]
    // The MAX life the ship has
    [SerializeField] float maxLife = 100f;
    public float MaxLife { get { return maxLife; } }

    // The MAX shield the ship has
    [SerializeField] float maxShield = 100f;
    public float MaxShield { get { return maxShield; } }

    [Space(10)]
    public float life;
    public float shield;

    [Space(10)]
    [Range(1, 99)]
    // The PERCENT of shield that is regenerated per second
    public int regenSpeed = 5;
    public float regenDelay = 1f;

    public bool invincible;

    public bool regen = true;

    // Start is called before the first frame update
    void Start()
    {
        life = maxLife;
        shield = maxShield;

        StartCoroutine(RegenDelayReset());
    }

    // Update is called once per frame
    void Update()
    {
        // If we have more then 0 life we can regen shields
        if (life > 0 && regen)
        {
            // Calculating the amount we need to heal WITH regen Speed
            float amountToHeal = shield + (maxShield * regenSpeed / 100f) * Time.deltaTime;

            // Clamp out shield to the max shield
            shield = Mathf.Clamp(amountToHeal, 0, maxShield);
        }
        else
        {
            OnDeath();
        }
    }

    // Damage the ship
    public void TakeDamage(float kineticDamage, float energyDamage )
    {
        if (!invincible)
        {
            regen = false;
            // Damage the shield
            shield = Mathf.Clamp(shield - energyDamage, 0, maxShield);

            // If we have negative shields we can take it away from your life pool
            if (shield == 0)
            {
                life -= kineticDamage;
            }

            // If we are dead cann OnDeath()
            if (life <= 0)
            {
                life = 0;
                StartCoroutine(OnDeath());
            }

            // TEMP -- COLOR THE THINGS YOU HIT
            //if (gameObject.GetComponent<Renderer>()) gameObject.GetComponent<Renderer>().material.SetColor("_BaseColor", Color.blue);
        }
    }

    // When life is 0 this is called by TakeDamage()
    IEnumerator OnDeath()
    {
        yield return new WaitForSeconds(0.1f);

        if (CompareTag("Player"))
        {
            gameObject.SetActive(false);

            life = maxLife;
            shield = maxShield;

            StartCoroutine(RegenDelayReset());
        }
        else
            Destroy(gameObject);
    }

    public void Heal(int amountToHeal)
    {
        life += amountToHeal;

        if (life > maxLife) life = maxLife;
    }

    IEnumerator RegenDelayReset()
    {
        while(true)
        {
            yield return new WaitForSeconds(regenDelay);

            regen = true;
        }
    }
}
