using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used to test crossfading of music manager mostly
/// </summary>
[RequireComponent(typeof(MusicManager))]
public class RandomizeMusic : MonoBehaviour
{
    [SerializeField] float interval = 10;

    private IEnumerator Start()
    {
        while(true)
        {
            yield return new WaitForSeconds(interval);
            int length = GetComponent<MusicManager>().Length;
            int rand = Random.Range(0, length);
            Debug.Log("TrackID: " + rand);
            GetComponent<MusicManager>().ChangeTrack(rand);
        }
    }
}
