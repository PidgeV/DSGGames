﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Area : MonoBehaviour
{
    public Transform objects;
    public Transform enemies;
    public Transform spawns;
    public Transform waypoints;
   
    [Range(1000, 5000)][SerializeField] private int size = 1000;
    [SerializeField] private int skyboxIndex;
    [Range(300, 1000)][SerializeField] private int spawnCheckRadius = 300;

    [SerializeField] private GameObject areaEffect;

    [SerializeField] private NodeSpawner spawner;

    void Awake()
    {
        if (!spawner)
            TryGetComponent(out spawner);

        gameObject.SetActive(false);

        LoadArea(false);

        areaEffect.transform.localScale = Vector3.one * size;

        foreach (Transform spawn in spawns)
        {
            spawn.LookAt(transform.position);
        }
    }

    public void LoadArea(bool load)
    {
        objects.gameObject.SetActive(load);
        enemies.gameObject.SetActive(load);
        areaEffect.gameObject.SetActive(load);
        spawner.enabled = load;
    }

    public void RespawnArea()
    {
        KillEnemies();

        foreach (Transform obj in objects)
        {
            Destroy(obj.gameObject);
        }

        GameObject dreadnova = GameObject.FindWithTag("CapitalShip");

        if (dreadnova && dreadnova.TryGetComponent(out DreadnovaController controller))
        {
            controller.ResetEnemy();
        }

        spawner.StartWave(0);
    }

    public void KillEnemies()
    {
        foreach (Transform enemy in enemies)
        {
            HealthAndShields[] health = enemy.GetComponentsInChildren<HealthAndShields>();

            foreach (HealthAndShields hs in health)
            {
                hs.TakeDamage(Mathf.Infinity, Mathf.Infinity);
            }
        }
    }

    public Transform FindSafeSpawn()
    {
        Transform[] sp = Spawns;

        int iterations = 0;
        int spawnIndex = Random.Range(0, sp.Length);

        while (true)
        {
            if (Physics.CheckSphere(sp[spawnIndex].position, spawnCheckRadius))
            {
                spawnIndex = (spawnIndex + 1) % sp.Length;
                iterations++;

                if (iterations >= sp.Length)
                {
                    return null;
                }
            }
            else
            {
                break;
            }
        }

        return sp[spawnIndex];
    }

    private Transform[] GetChildren(Transform parent)
    {
        Transform[] sp = new Transform[parent.childCount];

        for (int i = 0; i < sp.Length; i++)
        {
            sp[i] = parent.GetChild(i);
        }

        return sp;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, size);

        foreach(Transform spawn in spawns)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(spawn.position, 10f);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(spawn.position, spawnCheckRadius);
        }

        Gizmos.color = Color.blue;

        foreach (Transform waypoint in waypoints)
        {
            Gizmos.DrawWireSphere(waypoint.position, 10f);
        }
    }

    public Transform[] Objects { get { return GetChildren(objects); } }
    public Transform[] Enemies { get { return GetChildren(enemies); } }
    public Transform[] Spawns { get { return GetChildren(spawns); } }
    public Transform[] Waypoints { get { return GetChildren(waypoints); } }
    public int Size { get { return size; } }
    public int Skybox { get { return skyboxIndex; } }
    public bool IsPlayerOutside { get { return Vector3.Distance(GameManager.Instance.Player.transform.position, transform.position) >= size; } }
}
