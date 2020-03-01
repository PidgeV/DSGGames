using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MiniMapDistanceColor : MonoBehaviour
{
    [SerializeField] Color closeColor;
    [SerializeField] Color farColor;

    List<GameObject> minimapObjects = new List<GameObject>();

    // Start is called before the first frame update
    IEnumerator Start()
    {
        while(true)
        {
            yield return new WaitForSeconds(0.2f);

            foreach(GameObject go in minimapObjects)
            {
                float dist = Vector3.Distance(transform.position, go.transform.position);
                float farClip = GetComponent<Camera>().farClipPlane;

                if (dist <= farClip)
                {
                    float factor = dist / farClip;
                    Color newColor = Color.Lerp(farColor, closeColor, factor);
                    go.GetComponent<Renderer>().material.SetColor("_BaseColor", newColor);
                }
            }
        }
    }

    public void Add(GameObject minimapObject)
    {
        minimapObjects.Add(minimapObject);
    }

    public void Remove(GameObject minimapObject)
    {
        if (minimapObjects.Contains(minimapObject)) minimapObjects.Remove(minimapObject);
        else Debug.LogWarning("Object not in list");
    }
}
