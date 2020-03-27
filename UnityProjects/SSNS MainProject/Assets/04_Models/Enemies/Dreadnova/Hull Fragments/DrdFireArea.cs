using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrdFireArea : MonoBehaviour
{
    [SerializeField] Material glowy;
    [SerializeField] Material noGlowy;

    public void SetGlowy(bool glowing = true)
    {
        if(glowing)
        {
            gameObject.GetComponent<Renderer>().material = glowy;
        }
        else
        {
            gameObject.GetComponent<Renderer>().material = noGlowy;
        }
    }
}
