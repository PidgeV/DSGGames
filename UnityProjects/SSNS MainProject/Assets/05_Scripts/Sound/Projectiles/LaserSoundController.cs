using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This controller will change the audio source of the laser object based on when it is turned on or not.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class LaserSoundController : MonoBehaviour
{
    [SerializeField] AudioSource startSound;
    [SerializeField] AudioSource constantSound;
    [SerializeField] AudioSource disableSound;

    private void OnEnable()
    {
        if(constantSound) constantSound.Play();
        if(startSound) startSound.Play();
    }

    private void OnDisable()
    {
        if (disableSound) disableSound.Play();
    }
}
