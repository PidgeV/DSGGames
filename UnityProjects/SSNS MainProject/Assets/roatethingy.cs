using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.VFX;

public class roatethingy : MonoBehaviour
{
    [SerializeField] VisualEffect vfx;

    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(RotateAndStuff());
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StopEffect()
    {
        vfx.Stop();
    }

    IEnumerator RotateAndStuff()
    {
        bool test = false;

        while (true)
        {
            yield return null;
            if (!test) transform.Rotate(Vector3.up, 180 / 4 * Time.deltaTime);

            if (transform.rotation.y >= 0.85) vfx.Stop();
            else if (transform.rotation.y >= 0.95)
            {
                test = true;
                Debug.LogError("Tried stopping");
            }
        }


    }
}
