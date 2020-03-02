using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the soundtracks playing in game, their audio levels, and crossfading
/// </summary>
public class MusicManager : MonoBehaviour
{
    [SerializeField] AudioSource source1;
    [SerializeField] AudioSource source2;

    [SerializeField] AudioClip[] soundtracks;
    [SerializeField] int startingTrackNumber = 0;

    [Range(0.1f, 10f)]
    [SerializeField] float crossfadeSpeed = 1;

    [Range(0, 1)]
    [SerializeField] float maxVolume = 1;

    private bool usingSource1 = true;

    public int Length { get { return soundtracks.Length; } }

    // Start is called before the first frame update
    void Start()
    {
        if (startingTrackNumber < soundtracks.Length)
        {
            ChangeTrack(startingTrackNumber);
        }
        else
        {
            ChangeTrack(0);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ChangeTrack(int track)
    {
        StopCoroutine("CoChangeTrack");
        StartCoroutine(CoChangeTrack(track));
    }

    IEnumerator CoChangeTrack(int track)
    {
        if (track < soundtracks.Length)
        {
            if (usingSource1)
            {
                source1.clip = soundtracks[track];
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
                source2.clip = soundtracks[track];
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