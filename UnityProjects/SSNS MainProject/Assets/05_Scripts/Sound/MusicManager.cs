using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the soundtracks playing in game, their audio levels, and crossfading
/// </summary>
public class MusicManager : MonoBehaviour
{
    static public MusicManager instance;

    [SerializeField] AudioSource source1;
    [SerializeField] AudioSource source2;

    [SerializeField] AudioClip[] menuTracks;
    [SerializeField] AudioClip[] combatTracks;
    [SerializeField] AudioClip[] nonCombatTracks;

    [SerializeField] int startingTrackNumber = 0;

    [Range(0.1f, 10f)]
    [SerializeField] float crossfadeSpeed = 1;

    [Range(0, 1)]
    [SerializeField] float maxVolume = 1;

    private bool usingSource1 = true;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null) instance = this;

        if (startingTrackNumber < nonCombatTracks.Length)
        {
            ChangeTrack(SNSSTypes.MusicTrackType.MENU, startingTrackNumber);
        }
        else
        {
            ChangeTrack(SNSSTypes.MusicTrackType.MENU, 0);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Changes the track being played 
    /// </summary>
    /// <param name="musicType"></param>
    /// <param name="track"></param>
    public void ChangeTrack(SNSSTypes.MusicTrackType musicType, int track)
    {
        StopAllCoroutines();

        switch (musicType)
        {
            case SNSSTypes.MusicTrackType.MENU:
                StartCoroutine(StartMenuTrack(track));
                break;
            case SNSSTypes.MusicTrackType.NON_COMBAT:
                StartCoroutine(StartNonCombatTrack(track));
                break;
            case SNSSTypes.MusicTrackType.COMBAT:
                StartCoroutine(StartCombatTrack(track));
                break;
        }
    }

    IEnumerator StartMenuTrack(int track)
    {

        if (track < menuTracks.Length)
        {
            if (usingSource1)
            {
                source1.clip = menuTracks[track];
                source1.Play();

                while (source1.volume != maxVolume)
                {
                    yield return null;
                    source1.volume = Mathf.Clamp(source1.volume + (maxVolume / crossfadeSpeed * Time.deltaTime), 0, maxVolume);
                    source2.volume = Mathf.Clamp(source2.volume - (maxVolume / crossfadeSpeed * Time.deltaTime), 0, maxVolume);
                }
                source2.Stop();
            }
            else
            {
                source2.clip = menuTracks[track];
                source2.Play();

                while (source2.volume != maxVolume)
                {
                    yield return null;
                    source2.volume = Mathf.Clamp(source2.volume + (maxVolume / crossfadeSpeed * Time.deltaTime), 0, maxVolume);
                    source1.volume = Mathf.Clamp(source1.volume - (maxVolume / crossfadeSpeed * Time.deltaTime), 0, maxVolume);
                }
                source1.Stop();
            }
            usingSource1 = !usingSource1; //swapping what gets the new track
        }
        else
        {
            Debug.LogError("Sountrack desired does not exist.");
        }
    }

    IEnumerator StartNonCombatTrack(int track)
    {

        if (track < nonCombatTracks.Length)
        {
            if (usingSource1)
            {
                source1.clip = nonCombatTracks[track];
                source1.Play();

                while (source1.volume != maxVolume)
                {
                    yield return null;
                    source1.volume = Mathf.Clamp(source1.volume + (maxVolume / crossfadeSpeed * Time.deltaTime), 0, maxVolume);
                    source2.volume = Mathf.Clamp(source2.volume - (maxVolume / crossfadeSpeed * Time.deltaTime), 0, maxVolume);
                }
                source2.Stop();
            }
            else
            {
                source2.clip = nonCombatTracks[track];
                source2.Play();

                while (source2.volume != maxVolume)
                {
                    yield return null;
                    source2.volume = Mathf.Clamp(source2.volume + (maxVolume / crossfadeSpeed * Time.deltaTime), 0, maxVolume);
                    source1.volume = Mathf.Clamp(source1.volume - (maxVolume / crossfadeSpeed * Time.deltaTime), 0, maxVolume);
                }
                source1.Stop();
            }
            usingSource1 = !usingSource1; //swapping what gets the new track
        }
        else
        {
            Debug.LogError("Sountrack desired does not exist.");
        }
    }

    IEnumerator StartCombatTrack(int track)
    {

        if (track < combatTracks.Length)
        {
            if (usingSource1)
            {
                source1.clip = combatTracks[track];
                source1.Play();

                while (source1.volume != maxVolume)
                {
                    yield return null;
                    source1.volume = Mathf.Clamp(source1.volume + (maxVolume / crossfadeSpeed * Time.deltaTime), 0, maxVolume);
                    source2.volume = Mathf.Clamp(source2.volume - (maxVolume / crossfadeSpeed * Time.deltaTime), 0, maxVolume);
                }
                source2.Stop();
            }
            else
            {
                source2.clip = combatTracks[track];
                source2.Play();

                while (source2.volume != maxVolume)
                {
                    yield return null;
                    source2.volume = Mathf.Clamp(source2.volume + (maxVolume / crossfadeSpeed * Time.deltaTime), 0, maxVolume);
                    source1.volume = Mathf.Clamp(source1.volume - (maxVolume / crossfadeSpeed * Time.deltaTime), 0, maxVolume);
                }
                source1.Stop();
            }
            usingSource1 = !usingSource1; //swapping what gets the new track
        }
        else
        {
            Debug.LogError("Sountrack desired does not exist.");
        }
    }

}