using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This controller will change the audio source of the laser object based on when it is turned on or not.
/// </summary>
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(LaserBehaviour))]
public class LaserSoundController : MonoBehaviour
{
    [SerializeField] AudioSource otherSound;
    [SerializeField] AudioSource constantSound;

    [SerializeField] AudioClip startSound;
    [SerializeField] AudioClip disableSound;

    LaserBehaviour laser;
    bool startLaser = false;

    private void Start()
    {
        laser = GetComponent<LaserBehaviour>();
        StartCoroutine(FadeLaserSound());
    }

    private void Update()
    {
        if(laser.fadeIn != startLaser)
        {
            startLaser = laser.fadeIn;

            if (startLaser)
                otherSound.clip = startSound;
            else
                otherSound.clip = disableSound;

            otherSound.Play();
        }
    }

    IEnumerator FadeLaserSound()
    {

        while (true)
        {
            yield return null;
            if(startLaser)
            {
                constantSound.volume = Mathf.Clamp(constantSound.volume + 0.7f * Time.deltaTime, 0, 1);
            }
            else
            {
                constantSound.volume = Mathf.Clamp(constantSound.volume - 0.7f * Time.deltaTime, 0, 1);
            }
        }
    }
}
