using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TestAsteroid : MonoBehaviour
{
    public GameObject asteroidPrefab;
    public GameObject player;
    public int seed;
    public float minScale = 1;
    public float maxScale = 10;
    public float distanceBetween = 12;
    public int maxAsteroids = 10;
    public float minPos = -100;
    public float maxPos = 100;
    public float maxForce = 5;
    private GameObject[] asteroids;
    

    // Start is called before the first frame update
    void Start()
    {
        Random.InitState(seed);
        asteroids = new GameObject[maxAsteroids];

        bool foundPos = false;
        for (int i = 0; i < maxAsteroids; i++)
        {
            Vector3 pos = new Vector3();
            while (!foundPos)
            {
                foundPos = true;
                pos.x = Random.Range(minPos, maxPos);
                pos.y = Random.Range(minPos, maxPos);
                pos.z = Random.Range(minPos, maxPos);

                for (int j = 0; j < i; j++)
                {
                    if(ContainsPoint(pos, asteroids[j].transform.position))
                    {
                        foundPos = false;
                    }
                }
                if (ContainsPoint(pos, player.transform.position))
                {
                    foundPos = false;
                }

            }

            asteroids[i] = SpawnSingle(pos);
            foundPos = false;
        }
    }

    bool ContainsPoint(Vector3 pos1, Vector3 pos2)
    {
        bool contains = true;
        if(Mathf.Abs(pos1.x - pos2.x) > 12 && Mathf.Abs(pos1.y - pos2.y) > 12 && Mathf.Abs(pos1.z - pos2.z) > 12)
        {
            contains = false;
        }
        return contains;
    }

// Update is called once per frame
void Update()
    {
        
    }


    GameObject SpawnSingle(Vector3 position)
    {
        GameObject newAsteroid = Instantiate(asteroidPrefab);
        newAsteroid.transform.position = position;

        float scale = Random.Range(minScale, maxScale);
        Vector3 scale2 = new Vector3(Random.Range(scale - 0.5f, scale + 0.5f), Random.Range(scale - 0.5f, scale + 0.5f), Random.Range(scale - 0.5f, scale + 0.5f));
        Vector3 force = new Vector3(Random.Range(-maxForce, maxForce), Random.Range(-maxForce, maxForce), Random.Range(-maxForce, maxForce));

        newAsteroid.transform.localScale = scale2;
        newAsteroid.GetComponent<Rigidbody>().mass *= scale * scale;// * scale;
        newAsteroid.GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);

        return newAsteroid;
    }
}
