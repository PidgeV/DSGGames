using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SNSSTypes;

/// <summary>
/// ScriptableObject that holds information for generating areas
/// </summary>
[CreateAssetMenu(fileName = "Node Event", menuName = "Nodes/New Event")]
public class NodeEvent : ScriptableObject
{
    public NodeEventType eventType;

    [Tooltip("These prefabs are spawned and should include their own scripts for spawning stuff that reference AreaManager")]
    public GameObject[] prefabsToSpawn; 
}
