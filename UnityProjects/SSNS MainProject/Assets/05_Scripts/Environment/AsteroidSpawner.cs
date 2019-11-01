using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AsteroidSpawner : MonoBehaviour
{
    public GameObject asteroidPrefab;
	// public GameObject player;
    public int seed;
    public float minScale = 1;
    public float maxScale = 10;
    public int maxAsteroids = 10;   //Maximum asteroids in the field
    public float minPos = -100;
    public float maxPos = 100;
    public float maxForce = 5;
    private GameObject[] asteroids;     //All the asteroids
    private Mesh totalPlayer;   //Used to test intersection with the players starting position


    // Start is called before the first frame update
    void Start()
    {
        Random.InitState(seed);
        asteroids = new GameObject[maxAsteroids];
        bool foundPos;

        #region  Combine all of the meshes in the player so that we can check their bounds for new asteroids intersecting
        MeshFilter[] playerMeshFilters = GameObject.FindGameObjectWithTag("Player").GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[playerMeshFilters.Length];
        
        for (int i = 0; i < playerMeshFilters.Length; i++)
        {
            combine[i].mesh = playerMeshFilters[i].sharedMesh;
            combine[i].transform = playerMeshFilters[i].transform.localToWorldMatrix;
        }

        totalPlayer = new Mesh();
        totalPlayer.CombineMeshes(combine);
        #endregion


        for (int i = 0; i < maxAsteroids; i++)
        {
            Vector3 pos = new Vector3();
            foundPos = false;
            asteroids[i] = SpawnSingleAsteroid();

            //re-create new positons for the asteroid while we haven't found a valid one
            while (!foundPos)
            {
                foundPos = true;

                //Create the position for the asteroid
                pos.x = Random.Range(minPos, maxPos);
                pos.y = Random.Range(minPos, maxPos);
                pos.z = Random.Range(minPos, maxPos);
                asteroids[i].transform.position = pos;

                //Test if the asteroid would intersect with any existing asteroid
                for (int j = 0; j < i; j++)
                {
                    if (asteroids[j].GetComponent<MeshRenderer>().bounds.Intersects(asteroids[i].GetComponent<MeshRenderer>().bounds))
                    {
                        foundPos = false;
                    }
                }

                //Test if the asteroid would intersect with the player
                if (totalPlayer.bounds.Intersects(asteroids[i].GetComponent<MeshRenderer>().bounds))
                {
                    foundPos = false;
                }
            }
        }
    }

/// <summary>
/// Spawns and returns a single asteroid with randomized scale, force, mass
/// You must still set the position
/// </summary>
/// <returns></returns>
    GameObject SpawnSingleAsteroid()
    {
        GameObject newAsteroid = Instantiate(asteroidPrefab);

        float scale = Random.Range(minScale, maxScale);
        Vector3 scale2 = new Vector3(Random.Range(scale - 0.5f, scale + 0.5f), Random.Range(scale - 0.5f, scale + 0.5f), Random.Range(scale - 0.5f, scale + 0.5f));
        Vector3 force = new Vector3(Random.Range(-maxForce, maxForce), Random.Range(-maxForce, maxForce), Random.Range(-maxForce, maxForce));

        newAsteroid.transform.localScale = scale2;
        newAsteroid.GetComponent<Rigidbody>().mass *= scale * scale;// * scale;
        newAsteroid.GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);

        return newAsteroid;
    }
}
