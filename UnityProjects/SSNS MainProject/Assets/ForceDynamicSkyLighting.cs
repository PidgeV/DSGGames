using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.HDPipeline;

[RequireComponent(typeof(StaticLightingSky))]
public class ForceDynamicSkyLighting : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<StaticLightingSky>().dynamicLighting = true;
    }
}
