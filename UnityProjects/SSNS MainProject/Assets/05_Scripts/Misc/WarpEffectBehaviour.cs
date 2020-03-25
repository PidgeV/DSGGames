using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.VFX;

/// <summary>
/// Sets warp effect and transitions
/// </summary>
public class WarpEffectBehaviour : MonoBehaviour
{
    public static WarpEffectBehaviour instance;

    [SerializeField] float warpTransitionSpeed = 4f;
    VisualEffect vfx;
    float min = 1.2f;
    float max = 30f;
    bool warping = false;
    float lerpVal = 30f;
    // Start is called before the first frame update
    void Start()
    {
        if (!instance) instance = this;

        vfx = GetComponent<VisualEffect>();
        StartWarp();
    }

    // Update is called once per frame
    void Update()
    {
        if (warping && lerpVal != min)
        {
            lerpVal = Mathf.Clamp(lerpVal - warpTransitionSpeed * Time.deltaTime, min, max);
            vfx.SetFloat("WarpRadius", lerpVal);
        }
    }

    public void StartWarp()
    {
        warping = true;
        vfx.enabled = true;
    }

    public void EndWarp()
    {
        warping = false;
        vfx.SetFloat("WarpRadius", max);
        vfx.enabled = false;
        lerpVal = max;
    }
}
