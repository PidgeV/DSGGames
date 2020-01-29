using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Swarm/Behaviour/AvoidObstacles")]
public class AvoidObstacles : FlockBehaviour
{
    public override Vector3 CalculateMove(FlockAgent agent, List<Transform> context, Flock flock, List<Transform> obstacles)
    {
        //if no neighbours, return no adjustment
        if (obstacles.Count == 0) return Vector3.zero;

        //Add all the points and average
        Vector3 avoidanceMove = Vector3.zero;
        int nAvoid = 0;

        foreach (Transform t in obstacles)
        {
            nAvoid++;
            avoidanceMove += agent.transform.position - t.position;
        }

        if (nAvoid > 0) avoidanceMove /= nAvoid;

        return avoidanceMove;
    }
}
