﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Complete;

/// <summary>
/// The Main flock class used to spawn agents and leader.
/// Be sure to set the waypoints after spawning.
/// </summary>
public class Flock : MonoBehaviour
{

    public FlockAgent agentPrefab;
    public List<FlockAgent> agents = new List<FlockAgent>();
    public FlockBehaviour behaviour;

    [SerializeField] LayerMask agentLayer;
    [SerializeField] LayerMask obstacleLayers;

    #region Flock Leader Variables
    [SerializeField] GameObject flockLeaderPrefab;
    private GameObject flockLeader;
    
    public float swarmFollowRadius = 75f;

    [SerializeField] Transform[] waypoints;
    public Transform[] WayPoints { set { waypoints = value; } } // used to set waypoints when spawning swarm. Game Manager should set when spawning the object

    public GameObject defenseTarget; //If made null the swarm will enter patrol state and the leadre will act normally, if not null it will enter defense state.

    public Vector3 FlockLeaderPosition { get { return flockLeader.transform.position; } }
    #endregion

    #region Swarm Agent Variables
    [Range(1, 300)]
    public int startingCount = 250;
    const float agentDensity = 0.8f;

    [Range(1f, 1000f)]
    public float driveFactor = 10f;
    [Range(75f, 400f)]
    public float maxSpeed = 100f;
    [Range(10f, 250f)]
    public float neighbourRadius = 40f;
    [Range(1f, 50f)]
    public float avoidanceRadius = 20f;
    [Range(1, 200)]
    public float obstacleDistance = 50f;

    float sqrMaxSpeed;
    float sqrNeighbourRadius;
    float sqrAvoidanceRadius;
    public float SquareAvoidanceRadius { get { return sqrAvoidanceRadius; } }

    [HideInInspector] public GameObject player; //Remove at some point and look for the player status inside Game Manager. Or use a function from the Game Manager

    int incrementCount = 0;
    int incrementAmount = 100;

    public int SwarmCount { get { return agents.Count; } }

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        sqrMaxSpeed = Mathf.Pow(maxSpeed, 2);
        //sqrNeighbourRadius = Mathf.Pow(neighbourRadius, 2);
        sqrAvoidanceRadius = Mathf.Pow(avoidanceRadius, 2);

        //Spawn Leader
        flockLeader = Instantiate(
                flockLeaderPrefab,
                transform.position + Random.insideUnitSphere * startingCount * agentDensity,
                Quaternion.Euler(Vector3.forward * Random.Range(0f, 360f)),
                transform);
        flockLeader.GetComponent<FlockLeaderController>().waypoints = waypoints; //set waypoints of target leader
        flockLeader.transform.parent = transform;

        //Spawn all swarm agents
        for (int i = 0; i < startingCount; i++)
        {
            FlockAgent newAgent = Instantiate(
                agentPrefab,
                transform.position + Random.insideUnitSphere * startingCount * agentDensity,
                Quaternion.Euler(Vector3.forward * Random.Range(0f, 360f)),
                transform);
            newAgent.name = "Agent" + i;
            newAgent.Initialize(this);
            agents.Add(newAgent);
        }

        //Look for player
        player = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(DestroySwarm());
    }

    // Update is called once per frame
    void Update()
    {
        //target = player.transform.position;
        List<Transform> context = new List<Transform>();
        List<Transform> obstacles = new List<Transform>();
        Vector3 move = Vector3.zero;

        //Loop through each agent and run the behaviours
        foreach (FlockAgent agent in agents)
        {

            context = GetNearbyNeighbours(agent, context); //Use the physics engine to get nearby agents
            obstacles = GetNearbyObstacles(agent, obstacles);

            move = behaviour.CalculateMove(agent, context, this, obstacles); //Move the agent with the behaviour object

            move *= driveFactor; //Multiply the movement by the drive factor
            if (move.sqrMagnitude > sqrMaxSpeed)
            {
                move = move.normalized * maxSpeed; //lower speed to max speed
            }

            agent.Move(move); //Move agent
        }
        //Debug.Log("Movement: " + move);

        if (agents.Count == 0)
            Destroy(gameObject);
    }

    /// <summary>
    /// Gets a list of transforms for objects in the neighbourRadius on the specified layer. This will return the list of transforms inside the radius.
    /// It will also filter agents based on what swarm they are supposed to follow
    /// </summary>
    /// <param name="agent"></param>
    /// <returns></returns>
    List<Transform> GetNearbyNeighbours(FlockAgent agent, List<Transform> context)
    {
        context.Clear();

        Collider[] contextColliders = Physics.OverlapSphere(agent.transform.position, neighbourRadius, agentLayer);

        foreach (Collider c in contextColliders)
        {
            FlockAgent item = c.GetComponent<FlockAgent>();
            if (c != agent.AgentCollider && item != null && item.swarm == agent.swarm)
            {
                context.Add(c.transform);
            }
        }

        return context;
    }

    List<Transform> GetNearbyObstacles(FlockAgent agent, List<Transform> obstacles)
    {
        obstacles.Clear();

        Collider[] obstacleColliders = Physics.OverlapSphere(agent.transform.position, obstacleDistance, obstacleLayers);

        foreach(Collider c in obstacleColliders)
        {
            obstacles.Add(c.transform);
        }

        return obstacles;
    }

    IEnumerator DestroySwarm()
    {
        while (agents.Count > 0) yield return new WaitForSeconds(0.5f);

        Destroy(gameObject);
    }

}
