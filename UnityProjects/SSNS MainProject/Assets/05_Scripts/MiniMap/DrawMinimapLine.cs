using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawMinimapLine : MonoBehaviour
{
    [SerializeField] Color closeColor;
    [SerializeField] Color farColor;

    GameObject player;
    Camera minimapCamera;
    LineRenderer line;

    private void Start()
    {
        line = gameObject.AddComponent(typeof(LineRenderer)) as LineRenderer;
        line.positionCount = 3;
        line.startWidth = 5f;
        line.endWidth = 5f;

        line.startColor = Color.white;
    }

    // Update is called once per frame
    void Update()
    {
        bool onScreen = false;

        if (minimapCamera)
        {
            Vector3 coord = minimapCamera.WorldToViewportPoint(transform.position);

            if (coord.x > 0 && coord.y > 0 && coord.x < 1 && coord.y < 1) onScreen = true;
        }

        if (onScreen && player)
        {
            line.enabled = true;

            Vector3 direction = player.transform.position - transform.position;
            Vector3 pos = transform.position;

            Vector3[] points = new Vector3[3];
            points[0] = pos;
            points[1] = new Vector3(pos.x, pos.y + direction.y, pos.z);
            points[2] = player.transform.position;

            line.SetPositions(points);

            DepthColorChange();
        }
        else
        {
            line.enabled = false;

            if (!player)
            {
                player = GameObject.FindGameObjectWithTag("Player");
                if (player)
                {
                    minimapCamera = GameObject.FindGameObjectWithTag("MinimapCamera").GetComponent<Camera>();
                }
            }
        }
    }

    void DepthColorChange()
    {
        if(Vector3.Distance(transform.position, minimapCamera.transform.position) < Vector3.Distance(player.transform.position, minimapCamera.transform.position))
        {
            line.endColor = closeColor;
        }
        else
        {
            line.endColor = farColor;
        }
    }
}
