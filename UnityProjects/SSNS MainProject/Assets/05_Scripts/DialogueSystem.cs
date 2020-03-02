using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueSystem : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] Text dialogueText;
    [SerializeField] DialogueClass[] dialogue;

    Queue<DialogueClass> dialogueQueue = new Queue<DialogueClass>();
    // Start is called before the first frame update
    IEnumerator Start()
    {
        while(true)
        {
            yield return new WaitForSeconds(1);

            if (!audioSource) audioSource = GameObject.FindGameObjectWithTag("Dialogue").GetComponent<AudioSource>();

            if (audioSource && !audioSource.isPlaying && dialogueQueue.Count > 0)
            {
                audioSource.clip = dialogueQueue.Peek().soundClip;
                audioSource.Play();

                if (dialogueText) dialogueText.text = dialogueQueue.Peek().text;

                dialogueQueue.Dequeue();
            }
            else if(!audioSource.isPlaying && dialogueQueue.Count == 0)
            {
                dialogueText.text = "";
            }
            else if(!audioSource)
            {
                dialogueText.text = "";
                Debug.LogWarning("No AudioSource found for Dialogue");
            }
        }
    }

    public void AddDialogue(DialogueClass dialogue)
    {
        dialogueQueue.Enqueue(dialogue);
    }

    public void AddDialogue(int listIndex)
    {
        if(listIndex < dialogue.Length)
        {
            dialogueQueue.Enqueue(dialogue[listIndex]);
        }
        else
        {
            Debug.LogError("Index out of range.");
        }
    }
}
