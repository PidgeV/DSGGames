using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class TriggerAnim : MonoBehaviour
{
    [SerializeField] float triggerTime = 2f;

    Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();

        StartCoroutine(TriggerAnimation());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator TriggerAnimation()
    {
        yield return new WaitForSecondsRealtime(triggerTime);

        anim.SetTrigger("Start");
    }
}
