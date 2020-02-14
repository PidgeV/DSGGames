using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreCounter : MonoBehaviour
{
    [SerializeField] static int Score;
    [SerializeField] int thisScore;

    enum TypeOfHit { ShieldHit, DestroyHit };
    [SerializeField] TypeOfHit typeOfHit;

    [Header("Only Include if Neccessary")]
    //[Tooltip("Used if there is no HealthAndShields component on this object.")]
    //[SerializeField] GameObject goHit;
    [Tooltip("The UI element for the player text, only is used if tag is player")]
    [SerializeField] Text playerText;
    bool isPlayer = false;
    bool hitThisFrame = false;

    // Start is called before the first frame update
    void Start()
    {
        Score = 0;
        //if (typeOfHit == TypeOfHit.ShieldHit)
        //{
        //    if (goHit != null )//&& goHit.GetComponent<HealthAndShields>())
        //    {
        //        goHit.GetComponent<HealthAndShields>().onLifeChange += OnShieldHit;
        //        goHit.GetComponent<HealthAndShields>().onShieldChange += OnShieldHit;
        //    }
        //    else if(TryGetComponent(out HealthAndShields has))
        //    {
        //        has.GetComponent<HealthAndShields>().onLifeChange += OnShieldHit;
        //        has.GetComponent<HealthAndShields>().onShieldChange += OnShieldHit;
        //    }
        //}
        if (gameObject.tag == "Player")
        {
            isPlayer = true;
        }
    }

    private void Update()
    {
        if (isPlayer)
        {
            playerText.text = "Score: " + Score;
        }
        //hitThisFrame = false;
    }

    //void OnShieldHit(float current, float max)
    //{
    //    if (hitThisFrame == false && TryGetComponent(out ShotThing shot) && shot.whoSent == ShotThing.shotFrom.Player)
    //    {
    //        hitThisFrame = true;
    //        Score += thisScore;
    //    }
    //}

    /// <summary>
    /// Only call with null go if it's the laser shooting
    /// </summary>
    /// <param name="thingShot"></param>
    public void Hit(GameObject thingShot = null)
    {
        if (thingShot == null)
        {
            if (typeOfHit == TypeOfHit.ShieldHit)
            {
                Score += thisScore;
            }
        }
        else if (thingShot.TryGetComponent(out ShotThing st))
        {

            if (st.whoSent == ShotThing.shotFrom.Player && typeOfHit == TypeOfHit.ShieldHit)
            {
                Score += thisScore;
            }
        }

    }


    private void OnCollisionEnter(Collision collision)
    {
        Hit(collision.gameObject);
    }

    private void OnDestroy()
    {
        if (typeOfHit == TypeOfHit.DestroyHit)
        {
            Score += thisScore;
        }
    }
}
