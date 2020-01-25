using System.Collections;
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
    List<FlockAgent> agents = new List<FlockAgent>();
    public FlockBehaviour behaviour;

    [SerializeField] LayerMask agentLayer;

    #region Flock Leader Variables
    [SerializeField] GameObject flockLeaderPrefab;
    private GameObject flockLeader;
    public float swarmFollowRadius = 75f;

    [SerializeField] GameObject[] waypoints;
    public GameObject[] WayPoints { set { waypoints = value; } } // used to set waypoints when spawning swarm. Game Manager should set when spawning the object

    public Vector3 FlockLeaderPosition { get { return flockLeader.transform.position; } }
    #endregion

    #region Swarm Agent Variables
    [Range(1, 500)]
    public int startingCount = 250;
    const float agentDensity = 0.1f;

    [Range(1f, 1000f)]
    public float driveFactor = 10f;
    [Range(75f, 400f)]
    public float maxSpeed = 100f;
    [Range(10f, 250f)]
    public float neighbourRadius = 40f;
    [Range(1f, 50f)]
    public float avoidanceRadius = 20f;

    float sqrMaxSpeed;
    float sqrNeighbourRadius;
    float sqrAvoidanceRadius;
    public float SquareAvoidanceRadius { get { return sqrAvoidanceRadius; } }

    GameObject player;

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
            newAgent.transform.parent = transform;
            agents.Add(newAgent);
        }

        //Look for player
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        //target = player.transform.position;
        List<Transform> context = new List<Transform>();
        Vector3 move = Vector3.zero;



        foreach (FlockAgent agent in agents)
        {

            GetNearbyNeighbours(agent, context);

            move = behaviour.CalculateMove(agent, context, this);

            move *= driveFactor;
            if (move.sqrMagnitude > sqrMaxSpeed)
            {
                move = move.normalized * maxSpeed;
            }

            agent.Move(move);
        }
        //Debug.Log("Movement: " + move);
    }

    /// <summary>
    /// Gets a list of transforms for objects in the neighbourRadius on the specified layer. This will return the list of transforms inside the radius.
    /// </summary>
    /// <param name="agent"></param>
    /// <returns></returns>
    void GetNearbyNeighbours(FlockAgent agent, List<Transform> context)
    {
        //List<Transform> context = new List<Transform>();
        context.Clear();

        Collider[] contextColliders = Physics.OverlapSphere(agent.transform.position, neighbourRadius, agentLayer);

        foreach (Collider c in contextColliders)
        {
            if (c != agent.AgentCollider)
            {
                context.Add(c.transform);
            }
        }
    }

    public void DestroySwarm()
    {
        Destroy(gameObject);
    }
}
