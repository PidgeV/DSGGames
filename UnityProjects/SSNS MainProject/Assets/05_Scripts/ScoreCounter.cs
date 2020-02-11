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
    [Tooltip("The Game Obect that will be hit. Not used OnDestroy")]
    [SerializeField] GameObject goHit;
    [Tooltip("The UI element for the player text, only is used if tag is player")]
    [SerializeField] Text playerText;
    bool isPlayer = false;
    bool hitThisFrame = false;

    // Start is called before the first frame update
    void Start()
    {
        Score = 0;
        if (typeOfHit == TypeOfHit.ShieldHit)
        {
            if (goHit != null )//&& goHit.GetComponent<HealthAndShields>())
            {
                goHit.GetComponent<HealthAndShields>().onLifeChange += OnShieldHit;
                goHit.GetComponent<HealthAndShields>().onShieldChange += OnShieldHit;
            }
        }
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
        hitThisFrame = false;
    }

    void OnShieldHit(float current, float max)
    {
        if (hitThisFrame == false)
        {
            hitThisFrame = true;
            Score += thisScore;
        }
    }

    private void OnDestroy()
    {
        if (typeOfHit == TypeOfHit.DestroyHit)
        {
            Score += thisScore;
        }
    }
}
