using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Swarm/Behaviour/Stay In Radius")]
public class StayInRadiusBehaviour : FlockBehaviour
{
    public float radius = 10f;

    public override Vector3 CalculateMove(FlockAgent agent, List<Transform> context, Flock flock)
    {
        Vector3 target = flock.target;
        Vector3 centerOffset = target - agent.transform.position;
        float t = centerOffset.magnitude / radius;

        if (t < 0.9f)
        {
            //Debug.Log(t + ", " + centerOffset + ", " + center);
            return Vector3.zero;
        }

        return target * t * t;
    }
}
