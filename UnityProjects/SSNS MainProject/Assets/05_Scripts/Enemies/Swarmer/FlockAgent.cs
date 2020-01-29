using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.VFX;

[RequireComponent(typeof(Collider))]
public class FlockAgent : MonoBehaviour
{
    public Flock swarm;
    Collider agentCollider;
    public Collider AgentCollider { get { return agentCollider; } }
    [SerializeField] GameObject explosionVFXPrefab;

    // Start is called before the first frame update
    void Start()
    {
        agentCollider = GetComponent<Collider>();
    }

    public void Initialize(Flock swarmObj)
    {
        swarm = swarmObj;
        transform.parent = swarm.transform;
    }

    public void Move(Vector3 velocity)
    {
        transform.forward = velocity;// Vector3.Lerp(transform.forward, velocity, Time.deltaTime);
        transform.position += velocity * Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(explosionVFXPrefab != null) Instantiate(explosionVFXPrefab, transform.position, transform.rotation);
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        swarm.agents.Remove(this);
    }
}
